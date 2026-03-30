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

public class FundSourceExpenditureImportJob(
    ILogger<FundSourceExpenditureImportJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient,
    ArcGisAuthService arcGisAuthService,
    FinanceApiDownloadService financeApiDownloadService)
    : ScheduledBackgroundJobBase<FundSourceExpenditureImportJob>(JobName, logger, webHostEnvironment, dbContext,
        configuration, sitkaSmtpClient)
{
    public const string JobName = "FundSource Expenditure Import";

    private const int TableTypeID = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.FundSourceExpenditure;
    private const string ImportProc = "dbo.pArcOnlineFundSourceExpenditureImportJson";
    private const string OutFields = "FTE_AMOUNT,TAR_HR_AMOUNT,BIENNIUM,FISCAL_MONTH,FISCAL_ADJUSTMENT_MONTH,CALENDAR_YEAR,MONTH_NAME,SOURCE_SYSTEM,DOCUMENT_NUMBER,DOCUMENT_SUFFIX,DOCUMENT_DATE,DOCUMENT_INVOICE_NUMBER,INVOICE_DESCRIPTION,INVOICE_DATE,INVOICE_NUMBER,GL_ACCOUNT_NUMBER,OBJECT_CODE,OBJECT_NAME,SUB_OBJECT_CODE,SUB_OBJECT_NAME,SUB_SUB_OBJECT_CODE,SUB_SUB_OBJECT_NAME,APPROPRIATION_CODE,APPROPRIATION_NAME,FUND_CODE,FUND_NAME,ORG_CODE,ORG_NAME,PROGRAM_INDEX_CODE,PROGRAM_INDEX_NAME,PROGRAM_CODE,PROGRAM_NAME,SUB_PROGRAM_CODE,SUB_PROGRAM_NAME,ACTIVITY_CODE,ACTIVITY_NAME,SUB_ACTIVITY_CODE,SUB_ACTIVITY_NAME,PROJECT_CODE,PROJECT_NAME,VENDOR_NUMBER,VENDOR_NAME,EXPENDITURE_ACCURED,ENCUMBRANCE";

    private const int BeginBienniumFiscalYear = 2001;
    private const int BienniumStep = 2;

    public override List<RunEnvironment> RunEnvironments => new()
    {
        RunEnvironment.Production,
        RunEnvironment.Staging,
        //RunEnvironment.Development
    };

    protected override void RunJobImplementation()
    {
        DownloadAllBienniumsAsync().GetAwaiter().GetResult();
    }

    private async Task DownloadAllBienniumsAsync()
    {
        Logger.LogInformation("Starting {JobName} DownloadFundSourceExpendituresTableForAllFiscalYears", JobName);
        await financeApiDownloadService.ClearOutdatedImportsAsync();

        var token = await arcGisAuthService.GetDataImportUserTokenAsync();
        var lastLoadDate = await financeApiDownloadService.GetLastLoadDateAsync(token);

        var currentBiennium = await financeApiDownloadService.GetCurrentBienniumFiscalYearAsync();
        var endBienniumFiscalYear = currentBiennium + BienniumStep;

        for (var biennium = BeginBienniumFiscalYear; biennium <= endBienniumFiscalYear; biennium += BienniumStep)
        {
            try
            {
                await ImportExpendituresForBienniumAsync(biennium, lastLoadDate, token);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error importing Expenditures for Biennium Fiscal Year {Biennium}", biennium);
            }
        }

        Logger.LogInformation("Ending {JobName} DownloadFundSourceExpendituresTableForAllFiscalYears", JobName);
    }

    private async Task ImportExpendituresForBienniumAsync(int bienniumFiscalYear, DateTime lastLoadDate, string token)
    {
        Logger.LogInformation("ImportExpendituresForGivenBienniumFiscalYear - Biennium Fiscal Year {Biennium}", bienniumFiscalYear);

        var importInfo = await financeApiDownloadService.GetLatestSuccessfulImportAsync(TableTypeID, bienniumFiscalYear);
        if (importInfo != null && importInfo.FinanceApiLastLoadDate == lastLoadDate)
        {
            Logger.LogInformation("Biennium {Biennium} already current. Last import: {ImportDate} - LastFinanceApiLoadDate: {LoadDate}",
                bienniumFiscalYear, importInfo.JsonImportDate, lastLoadDate);
            return;
        }

        await financeApiDownloadService.ClearFundSourceAllocationExpenditureTablesAsync(bienniumFiscalYear);

        var whereClause = $"BIENNIUM='{bienniumFiscalYear}'";
        var json = await financeApiDownloadService.DownloadPaginatedJsonAsync(
            WADNRConfiguration.FundSourceExpendituresJsonApiBaseUrl, token, whereClause, OutFields, "");
        Logger.LogInformation("FundSourceExpenditure BienniumFiscalYear {Biennium} JSON length: {Length}",
            bienniumFiscalYear, json.Length);

        var importID = await financeApiDownloadService.StoreRawJsonImportAsync(TableTypeID, lastLoadDate, bienniumFiscalYear, json);
        Logger.LogInformation("New ArcOnlineFinanceApiRawJsonImportID: {ImportID}", importID);

        try
        {
            await financeApiDownloadService.ExecuteImportProcAsync(ImportProc, importID, bienniumFiscalYear);
        }
        catch (Exception e)
        {
            await financeApiDownloadService.MarkImportStatusAsync(importID, JsonImportStatusTypeEnum.ProcessingFailed);
            throw new ApplicationException(
                $"ImportExpendituresForGivenBienniumFiscalYear failed for ArcOnlineFinanceApiRawJsonImportID {importID}", e);
        }

        await financeApiDownloadService.MarkImportStatusAsync(importID, JsonImportStatusTypeEnum.ProcessingSuceeded);
    }
}
