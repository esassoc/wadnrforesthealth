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

public class ProjectCodeImportJob(
    ILogger<ProjectCodeImportJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient,
    ArcGisAuthService arcGisAuthService,
    FinanceApiDownloadService financeApiDownloadService)
    : ScheduledBackgroundJobBase<ProjectCodeImportJob>(JobName, logger, webHostEnvironment, dbContext,
        configuration, sitkaSmtpClient)
{
    public const string JobName = "Project Code Import";

    private const int TableTypeID = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.ProjectCode;
    private const string ImportProc = "dbo.pArcOnlineProjectCodeImportJson";
    private const string OutFields = "PROJECT_END_DATE,CREATE_DATE,PROJECT_CODE,TITLE,PROJECT_START_DATE";
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
        Logger.LogInformation("Starting {JobName} DownloadArcOnlineFinanceApiProjectCodeTable", JobName);
        await financeApiDownloadService.ClearOutdatedImportsAsync();

        var token = await arcGisAuthService.GetDataImportUserTokenAsync();
        var lastLoadDate = await financeApiDownloadService.GetLastLoadDateAsync(token);

        var importInfo = await financeApiDownloadService.GetLatestSuccessfulImportAsync(TableTypeID, null);
        if (importInfo != null && importInfo.FinanceApiLastLoadDate == lastLoadDate)
        {
            Logger.LogInformation("ProjectCode table already current. Last import: {ImportDate} - LastFinanceApiLoadDate: {LoadDate}",
                importInfo.JsonImportDate, lastLoadDate);
            return;
        }

        var json = await financeApiDownloadService.DownloadPaginatedJsonAsync(
            WADNRConfiguration.ProjectCodeJsonApiBaseUrl, token, WhereClause, OutFields, "");
        Logger.LogInformation("ProjectCode JSON length: {Length}", json.Length);

        var importID = await financeApiDownloadService.StoreRawJsonImportAsync(TableTypeID, lastLoadDate, null, json);
        Logger.LogInformation("New ArcOnlineFinanceApiRawJsonImportID: {ImportID}", importID);

        try
        {
            await financeApiDownloadService.ExecuteImportProcAsync(ImportProc, importID);
        }
        catch (Exception e)
        {
            await financeApiDownloadService.MarkImportStatusAsync(importID, JsonImportStatusTypeEnum.ProcessingFailed);
            throw new ApplicationException($"ProjectCodeImportJson failed for ArcOnlineFinanceApiRawJsonImportID {importID}", e);
        }

        await financeApiDownloadService.MarkImportStatusAsync(importID, JsonImportStatusTypeEnum.ProcessingSuceeded);
        Logger.LogInformation("Ending {JobName} DownloadArcOnlineFinanceApiProjectCodeTable", JobName);
    }
}
