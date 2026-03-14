using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Hangfire;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.Job;

namespace WADNR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController(
    WADNRDbContext dbContext,
    ILogger<JobController> logger,
    IOptions<WADNRConfiguration> configuration,
    FinanceApiDownloadService financeApiDownloadService,
    ArcGisAuthService arcGisAuthService)
    : SitkaController<JobController>(dbContext, logger, configuration)
{
    /// <summary>
    /// Triggers a recurring Hangfire job immediately by name.
    /// </summary>
    [HttpPost("{jobName}/trigger")]
    [AdminFeature]
    public ActionResult TriggerJob([FromRoute] string jobName)
    {
        try
        {
            HangfireJobScheduler.EnqueueRecurringJob(jobName);
            return Ok(new { Message = $"Job '{jobName}' has been enqueued." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to trigger job {JobName}", jobName);
            return BadRequest(new { Message = $"Failed to trigger job '{jobName}': {ex.Message}" });
        }
    }

    /// <summary>
    /// Returns ArcOnlineFinanceApiRawJsonImport records for monitoring.
    /// </summary>
    [HttpGet("import-history")]
    [AdminFeature]
    public async Task<ActionResult<List<ImportHistory>>> GetImportHistory()
    {
        var history = await DbContext.ArcOnlineFinanceApiRawJsonImports
            .AsNoTracking()
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new ImportHistory
            {
                ArcOnlineFinanceApiRawJsonImportID = x.ArcOnlineFinanceApiRawJsonImportID,
                CreateDate = x.CreateDate,
                ArcOnlineFinanceApiRawJsonImportTableTypeID = x.ArcOnlineFinanceApiRawJsonImportTableTypeID,
                BienniumFiscalYear = x.BienniumFiscalYear,
                FinanceApiLastLoadDate = x.FinanceApiLastLoadDate,
                JsonImportDate = x.JsonImportDate,
                JsonImportStatusTypeID = x.JsonImportStatusTypeID,
                RawJsonStringLength = (long?)EF.Functions.DataLength(x.RawJsonString)
            })
            .ToListAsync();

        foreach (var item in history)
        {
            if (ArcOnlineFinanceApiRawJsonImportTableType.AllLookupDictionary.TryGetValue(item.ArcOnlineFinanceApiRawJsonImportTableTypeID, out var tableType))
            {
                item.ArcOnlineFinanceApiRawJsonImportTableTypeName = tableType.ArcOnlineFinanceApiRawJsonImportTableTypeName;
            }

            if (JsonImportStatusType.AllLookupDictionary.TryGetValue(item.JsonImportStatusTypeID, out var statusType))
            {
                item.JsonImportStatusTypeName = statusType.JsonImportStatusTypeName;
            }
        }

        return Ok(history);
    }

    /// <summary>
    /// Returns freshness info for a given import job by comparing the local DB's latest
    /// successful import against the external Finance API's current LOAD_COMPLETE_DATE.
    /// </summary>
    [HttpGet("{jobName}/freshness")]
    [AdminFeature]
    public async Task<ActionResult> CheckFreshness([FromRoute] string jobName)
    {
        if (!JobNameToTableTypeID.TryGetValue(jobName, out var tableTypeID))
        {
            return BadRequest(new { Message = $"Unknown job name '{jobName}'." });
        }

        try
        {
            var token = await arcGisAuthService.GetDataImportUserTokenAsync();
            var externalLastLoadDate = await financeApiDownloadService.GetLastLoadDateAsync(token);
            var importInfo = await financeApiDownloadService.GetLatestSuccessfulImportAsync(tableTypeID, null);

            var isCurrent = importInfo != null && importInfo.FinanceApiLastLoadDate == externalLastLoadDate;

            return Ok(new
            {
                IsCurrent = isCurrent,
                LastImportDate = importInfo?.JsonImportDate,
                LastLoadDate = importInfo?.FinanceApiLastLoadDate,
                LatestAvailableLoadDate = externalLastLoadDate
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to check freshness for job {JobName}", jobName);
            return StatusCode(500, new { Message = $"Unable to check freshness: {ex.Message}" });
        }
    }

    private static readonly Dictionary<string, int> JobNameToTableTypeID = new()
    {
        ["Vendor Import"] = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.Vendor,
        ["Program Index Import"] = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.ProgramIndex,
        ["Project Code Import"] = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.ProjectCode,
        ["FundSource Expenditure Import"] = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.FundSourceExpenditure,
    };

    /// <summary>
    /// Clears outdated ArcOnline finance API raw JSON imports.
    /// </summary>
    [HttpPost("clear-outdated-imports")]
    [AdminFeature]
    public async Task<ActionResult> ClearOutdatedImports()
    {
        await DbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.pClearOutdatedArcOnlineFinanceApiRawJsonImports @daysOldToRemove = {0}", 5);
        return Ok(new { Message = "Outdated imports cleared." });
    }

}
