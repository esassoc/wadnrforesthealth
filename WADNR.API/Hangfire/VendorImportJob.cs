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

public class VendorImportJob(
    ILogger<VendorImportJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient,
    ArcGisAuthService arcGisAuthService,
    FinanceApiDownloadService financeApiDownloadService)
    : ScheduledBackgroundJobBase<VendorImportJob>(JobName, logger, webHostEnvironment, dbContext,
        configuration, sitkaSmtpClient)
{
    public const string JobName = "Vendor Import";

    private const int TableTypeID = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.Vendor;
    private const string ImportProc = "dbo.pArcOnlineVendorImportJson";
    private const string OutFields = "REMARKS,LAST_PROCESS_DATE,VENDOR_NUMBER,VENDOR_NUMBER_SUFFIX,VENDOR_NAME,ADDRESS_LINE1,ADDRESS_LINE2,ADDRESS_LINE3,CITY,STATE,ZIP_CODE,ZIP_PLUS_4,PHONE_NUMBER,VENDOR_STATUS,VENDOR_TYPE,BILLING_AGENCY,BILLING_SUBAGENCY,BILLING_FUND,BILLING_FUND_BREAKOUT,CCD_CTX_FLAG,EMAIL";
    private const string WhereClause = "VENDOR_STATUS='A'";

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
        Logger.LogInformation("Starting {JobName} DownloadArcOnlineVendorTable", JobName);
        await financeApiDownloadService.ClearOutdatedImportsAsync();

        var token = await arcGisAuthService.GetDataImportUserTokenAsync();
        var lastLoadDate = await financeApiDownloadService.GetLastLoadDateAsync(token);

        var importInfo = await financeApiDownloadService.GetLatestSuccessfulImportAsync(TableTypeID, null);
        if (importInfo != null && importInfo.FinanceApiLastLoadDate == lastLoadDate)
        {
            Logger.LogInformation("Vendor table already current. Last import: {ImportDate} - LastFinanceApiLoadDate: {LoadDate}",
                importInfo.JsonImportDate, lastLoadDate);
            return;
        }

        var json = await financeApiDownloadService.DownloadPaginatedJsonAsync(
            WADNRConfiguration.VendorJsonApiBaseUrl, token, WhereClause, OutFields, "");
        Logger.LogInformation("Vendor JSON length: {Length}", json.Length);

        var importID = await financeApiDownloadService.StoreRawJsonImportAsync(TableTypeID, lastLoadDate, null, json);
        Logger.LogInformation("New ArcOnlineFinanceApiRawJsonImportID: {ImportID}", importID);

        try
        {
            await financeApiDownloadService.ExecuteImportProcAsync(ImportProc, importID);
        }
        catch (Exception e)
        {
            await financeApiDownloadService.MarkImportStatusAsync(importID, JsonImportStatusTypeEnum.ProcessingFailed);
            throw new ApplicationException($"ArcOnlineVendorImportJson failed for ArcOnlineFinanceApiRawJsonImportID {importID}", e);
        }

        await financeApiDownloadService.MarkImportStatusAsync(importID, JsonImportStatusTypeEnum.ProcessingSuceeded);
        Logger.LogInformation("Ending {JobName} DownloadArcOnlineVendorTable", JobName);
    }
}
