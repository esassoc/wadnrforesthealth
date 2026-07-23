using Microsoft.EntityFrameworkCore;
using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects.ServiceForestryUpload;

namespace WADNR.EFModels.Entities;

public static class ServiceForestryUploads
{
    public static async Task<ServiceForestryUploadDashboard> GetDashboardAsync(WADNRDbContext dbContext)
    {
        var latestImport = await dbContext.TabularDataImports
            .AsNoTracking()
            .Where(x => x.TabularDataImportTableTypeID == (int)TabularDataImportTableTypeEnum.ServiceForestry)
            .OrderByDescending(x => x.UploadDate)
            .Select(LoaUploadProjections.AsGridRow)
            .FirstOrDefaultAsync();

        // Processing is needed when the latest import has not yet been processed.
        var publishingNeeded = latestImport != null && latestImport.LastProcessedDate == null;

        return new ServiceForestryUploadDashboard
        {
            LatestImport = latestImport,
            PublishingProcessingIsNeeded = publishingNeeded,
        };
    }

    public static async Task<ServiceForestryUploadResult> ImportServiceForestryFileAsync(
        WADNRDbContext dbContext, Stream fileStream, int uploadPersonID)
    {
        var startTime = DateTime.Now;
        var errorList = new List<string>();

        var parsedRows = ServiceForestryExcelParser.ParseExcelFile(fileStream, errorList);

        // Replace the previous staging snapshot entirely.
        var previousRecords = await dbContext.ServiceForestryStages.ToListAsync();
        dbContext.ServiceForestryStages.RemoveRange(previousRecords);

        var countAdded = 0;
        foreach (var row in parsedRows)
        {
            if (string.IsNullOrEmpty(row.ProjectIdentifier))
            {
                continue;
            }

            var stage = new ServiceForestryStage
            {
                RegionTitle = row.RegionTitle,
                ProjectIdentifier = row.ProjectIdentifier,
                ApprovalDate = ToDateOnly(row.ApprovalDate),
                County = row.County,
                Forester = row.Forester,
                TotalAcres = row.TotalAcres,
                StewardshipPlan = row.StewardshipPlan,
                PercentMatch = row.PercentMatch,
                FundSource = row.FundSource,
                DCStatus = row.DCStatus,
                DCAllocatedAmount = row.DCAllocatedAmount,
                DCLetterDate = ToDateOnly(row.DCLetterDate),
                DCExpirationDate = ToDateOnly(row.DCExpirationDate),
                DCTreatment1 = row.DCTreatments[0],
                DCCost1 = row.DCCosts[0],
                DCCostPerAcre1 = row.DCCostPerAcres[0],
                DCAcresTreatment1 = row.DCAcresTreatments[0],
                DCTreatment2 = row.DCTreatments[1],
                DCCost2 = row.DCCosts[1],
                DCCostPerAcre2 = row.DCCostPerAcres[1],
                DCAcresTreatment2 = row.DCAcresTreatments[1],
                DCTreatment3 = row.DCTreatments[2],
                DCCost3 = row.DCCosts[2],
                DCCostPerAcre3 = row.DCCostPerAcres[2],
                DCAcresTreatment3 = row.DCAcresTreatments[2],
                DCTreatment4 = row.DCTreatments[3],
                DCCost4 = row.DCCosts[3],
                DCCostPerAcre4 = row.DCCostPerAcres[3],
                DCAcresTreatment4 = row.DCAcresTreatments[3],
                DCTreatment5 = row.DCTreatments[4],
                DCCost5 = row.DCCosts[4],
                DCCostPerAcre5 = row.DCCostPerAcres[4],
                DCAcresTreatment5 = row.DCAcresTreatments[4],
                DCTreatment6 = row.DCTreatments[5],
                DCCost6 = row.DCCosts[5],
                DCCostPerAcre6 = row.DCCostPerAcres[5],
                DCAcresTreatment6 = row.DCAcresTreatments[5],
                DCTotalMaxAmount = row.DCTotalMaxAmount,
                DCTreatedAcres = row.DCTreatedAcres,
                DCContractor = row.DCContractor,
                DCVendorName1 = row.DCVendorName1,
                DCVendorName2 = row.DCVendorName2,
                DCVendorAddress1 = row.DCVendorAddress1,
                DCVendorAddress2 = row.DCVendorAddress2,
                DCSwvVendorNumber = row.DCSwvVendorNumber,
                DCInvoiceDate = ToDateOnly(row.DCInvoiceDate),
                DCProgramIndex = row.DCProgramIndex,
                DCProjectCode = row.DCProjectCode,
                DCMatchAmount = row.DCMatchAmount,
                DCPayAmount = row.DCPayAmount,
                ItemType = row.ItemType,
                SourcePath = row.SourcePath,
            };

            dbContext.ServiceForestryStages.Add(stage);
            countAdded++;
        }

        await dbContext.SaveChangesAsync();

        var tabularDataImport = new TabularDataImport
        {
            TabularDataImportTableTypeID = (int)TabularDataImportTableTypeEnum.ServiceForestry,
            UploadDate = DateTime.Now,
            UploadPersonID = uploadPersonID,
        };
        dbContext.TabularDataImports.Add(tabularDataImport);
        await dbContext.SaveChangesAsync();

        var elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;

        return new ServiceForestryUploadResult
        {
            RecordsImported = countAdded,
            ElapsedSeconds = elapsedSeconds,
            Warnings = errorList,
        };
    }

    public static async Task<ServiceForestryPublishingResult> RunPublishingProcessingAsync(
        WADNRDbContext dbContext, int personID)
    {
        var startTime = DateTime.Now;
        try
        {
            // Long-running SP allowance, matching the LOA publishing process.
            dbContext.Database.SetCommandTimeout(400);
            await dbContext.Database.ExecuteSqlRawAsync("EXEC dbo.pImportServiceForestryTabularData");

            var processedDateTime = DateTime.Now;
            var latestImport = await dbContext.TabularDataImports
                .Where(x => x.TabularDataImportTableTypeID == (int)TabularDataImportTableTypeEnum.ServiceForestry)
                .OrderByDescending(x => x.UploadDate)
                .FirstOrDefaultAsync();

            if (latestImport != null)
            {
                latestImport.LastProcessedDate = processedDateTime;
                latestImport.LastProcessedPersonID = personID;
                await dbContext.SaveChangesAsync();
            }

            var elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
            return new ServiceForestryPublishingResult
            {
                Success = true,
                ElapsedSeconds = elapsedSeconds,
            };
        }
        catch (Exception ex)
        {
            var elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
            return new ServiceForestryPublishingResult
            {
                Success = false,
                ElapsedSeconds = elapsedSeconds,
                ErrorMessage = $"Problem executing Publishing: {ex.Message}",
            };
        }
    }

    private static DateOnly? ToDateOnly(DateTime? value) =>
        value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
}
