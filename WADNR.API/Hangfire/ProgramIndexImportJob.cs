using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.Common.EMail;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WADNR.API.Hangfire;

public class ProgramIndexImportJob(
    ILogger<ProgramIndexImportJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient,
    ArcGisAuthService arcGisAuthService,
    FinanceApiDownloadService financeApiDownloadService)
    : ScheduledBackgroundJobBase<ProgramIndexImportJob>(JobName, logger, webHostEnvironment, dbContext,
        configuration, sitkaSmtpClient)
{
    public const string JobName = "Program Index Import";

    private const int TableTypeID = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.ProgramIndex;
    private const string ImportProc = "dbo.pArcOnlineProgramIndexImportJson";
    private const string OutFields = "ACTIVITY_CODE,SUB_ACTIVITY_CODE,BIENNIUM,PROGRAM_INDEX_CODE,TITLE,PROGRAM_CODE,SUB_PROGRAM_CODE";
    private const string WhereClause = "1=1";

    public override List<RunEnvironment> RunEnvironments => new()
    {
        RunEnvironment.Production,
        RunEnvironment.Staging,
        //RunEnvironment.Development
    };

    protected override void RunJobImplementation()
    {
        DownloadAndImportAsync().GetAwaiter().GetResult();
    }

    private async Task DownloadAndImportAsync()
    {
        Logger.LogInformation("Starting {JobName} DownloadArcOnlineProgramIndexTable", JobName);
        await financeApiDownloadService.ClearOutdatedImportsAsync();

        var token = await arcGisAuthService.GetDataImportUserTokenAsync();
        var lastLoadDate = await financeApiDownloadService.GetLastLoadDateAsync(token);

        var importInfo = await financeApiDownloadService.GetLatestSuccessfulImportAsync(TableTypeID, null);
        if (importInfo != null && importInfo.FinanceApiLastLoadDate == lastLoadDate)
        {
            Logger.LogInformation("ProgramIndex table already current. Last import: {ImportDate} - LastFinanceApiLoadDate: {LoadDate}",
                importInfo.JsonImportDate, lastLoadDate);
            return;
        }

        var json = await financeApiDownloadService.DownloadPaginatedJsonAsync(
            WADNRConfiguration.ProgramIndexJsonApiBaseUrl, token, WhereClause, OutFields, "");
        Logger.LogInformation("ProgramIndex JSON length: {Length}", json.Length);

        var importID = await financeApiDownloadService.StoreRawJsonImportAsync(TableTypeID, lastLoadDate, null, json);
        Logger.LogInformation("New ArcOnlineFinanceApiRawJsonImportID: {ImportID}", importID);

        try
        {
            await financeApiDownloadService.ExecuteImportProcAsync(ImportProc, importID);
        }
        catch (Exception e)
        {
            await financeApiDownloadService.MarkImportStatusAsync(importID, JsonImportStatusTypeEnum.ProcessingFailed);
            throw new ApplicationException($"ProgramIndexImportJson failed for ArcOnlineFinanceApiRawJsonImportID {importID}", e);
        }

        await financeApiDownloadService.MarkImportStatusAsync(importID, JsonImportStatusTypeEnum.ProcessingSuceeded);
        Logger.LogInformation("Ending {JobName} DownloadArcOnlineProgramIndexTable", JobName);
    }
}
