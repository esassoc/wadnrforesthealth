using Microsoft.EntityFrameworkCore;
using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects.LoaUpload;

namespace WADNR.EFModels.Entities;

public static class LoaUploads
{
    public static async Task<LoaUploadDashboard> GetDashboardAsync(WADNRDbContext dbContext)
    {
        var latestNortheast = await dbContext.TabularDataImports
            .AsNoTracking()
            .Where(x => x.TabularDataImportTableTypeID == (int)TabularDataImportTableTypeEnum.LoaNortheast)
            .OrderByDescending(x => x.UploadDate)
            .Select(LoaUploadProjections.AsGridRow)
            .FirstOrDefaultAsync();

        var latestSoutheast = await dbContext.TabularDataImports
            .AsNoTracking()
            .Where(x => x.TabularDataImportTableTypeID == (int)TabularDataImportTableTypeEnum.LoaSoutheast)
            .OrderByDescending(x => x.UploadDate)
            .Select(LoaUploadProjections.AsGridRow)
            .FirstOrDefaultAsync();

        // Legacy logic: processing is needed when LastProcessedDate is null on the latest import
        var publishingNeeded = (latestNortheast != null && latestNortheast.LastProcessedDate == null) ||
                               (latestSoutheast != null && latestSoutheast.LastProcessedDate == null);

        return new LoaUploadDashboard
        {
            LatestNortheastImport = latestNortheast,
            LatestSoutheastImport = latestSoutheast,
            PublishingProcessingIsNeeded = publishingNeeded,
        };
    }

    public static async Task<LoaUploadResult> ImportLoaFileAsync(WADNRDbContext dbContext, Stream fileStream, bool isNortheast, int uploadPersonID)
    {
        var startTime = DateTime.Now;
        var errorList = new List<string>();

        var parsedRows = LoaExcelParser.ParseExcelFile(fileStream, errorList);

        // Delete previous staging records for this region
        if (isNortheast)
        {
            var previousRecords = await dbContext.LoaStages.Where(x => x.IsNortheast).ToListAsync();
            dbContext.LoaStages.RemoveRange(previousRecords);
        }
        else
        {
            var previousRecords = await dbContext.LoaStages.Where(x => !x.IsNortheast).ToListAsync();
            dbContext.LoaStages.RemoveRange(previousRecords);
        }

        // Insert new staging records
        var countAdded = 0;
        foreach (var row in parsedRows)
        {
            if (string.IsNullOrEmpty(row.ProjectID))
            {
                continue;
            }

            var loaStage = new LoaStage
            {
                ProjectIdentifier = row.ProjectID,
                ProjectStatus = row.Status,
                MatchAmount = row.MatchAmount.HasValue ? (decimal)row.MatchAmount.Value : null,
                PayAmount = row.PayAmount.HasValue ? (decimal)row.PayAmount.Value : null,
                FundSourceNumber = row.FundSourceNumber,
                FocusAreaName = row.FocusArea,
                ProjectExpirationDate = row.ProjectExpirationDate,
                LetterDate = row.LetterDate,
                ProgramIndex = row.ProgramIndex,
                ProjectCode = row.ProjectCode,
                IsNortheast = isNortheast,
                // IsSoutheast is a computed column — do not set
                ApplicationDate = row.ApplicationDate,
                DecisionDate = row.DecisionDate,
            };

            // Split forester name: "John Van Dyke" → first="John", last="VanDyke"
            var foresterParts = string.IsNullOrEmpty(row.Forester)
                ? Array.Empty<string>()
                : row.Forester.Split(' ');
            if (foresterParts.Length > 1)
            {
                loaStage.ForesterFirstName = foresterParts[0];
                loaStage.ForesterLastName = foresterParts[1];
                for (var i = 2; i < foresterParts.Length; i++)
                {
                    loaStage.ForesterLastName += foresterParts[i];
                }
                loaStage.ForesterEmail = row.ForesterEmail;
                loaStage.ForesterPhone = row.ForesterPhone;
            }

            dbContext.LoaStages.Add(loaStage);
            countAdded++;
        }

        await dbContext.SaveChangesAsync();

        // Record the upload in TabularDataImports
        var tabularDataImportTableTypeID = isNortheast
            ? (int)TabularDataImportTableTypeEnum.LoaNortheast
            : (int)TabularDataImportTableTypeEnum.LoaSoutheast;

        var tabularDataImport = new TabularDataImport
        {
            TabularDataImportTableTypeID = tabularDataImportTableTypeID,
            UploadDate = DateTime.Now,
            UploadPersonID = uploadPersonID,
        };
        dbContext.TabularDataImports.Add(tabularDataImport);
        await dbContext.SaveChangesAsync();

        var elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;

        return new LoaUploadResult
        {
            RecordsImported = countAdded,
            ElapsedSeconds = elapsedSeconds,
            Warnings = errorList,
        };
    }

    public static async Task<LoaPublishingResult> RunPublishingProcessingAsync(WADNRDbContext dbContext, int personID)
    {
        var startTime = DateTime.Now;
        try
        {
            // Set command timeout to 400 seconds (legacy requirement — SP can be long-running)
            dbContext.Database.SetCommandTimeout(400);
            await dbContext.Database.ExecuteSqlRawAsync("EXEC dbo.pImportLoaTabularData");

            // Update LastProcessedDate and LastProcessedPersonID on latest imports
            var processedDateTime = DateTime.Now;
            var imports = await dbContext.TabularDataImports.ToListAsync();

            var latestNortheast = imports
                .Where(x => x.TabularDataImportTableTypeID == (int)TabularDataImportTableTypeEnum.LoaNortheast)
                .OrderByDescending(x => x.UploadDate)
                .FirstOrDefault();

            var latestSoutheast = imports
                .Where(x => x.TabularDataImportTableTypeID == (int)TabularDataImportTableTypeEnum.LoaSoutheast)
                .OrderByDescending(x => x.UploadDate)
                .FirstOrDefault();

            if (latestNortheast != null)
            {
                latestNortheast.LastProcessedDate = processedDateTime;
                latestNortheast.LastProcessedPersonID = personID;
            }

            if (latestSoutheast != null)
            {
                latestSoutheast.LastProcessedDate = processedDateTime;
                latestSoutheast.LastProcessedPersonID = personID;
            }

            await dbContext.SaveChangesAsync();

            var elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
            return new LoaPublishingResult
            {
                Success = true,
                ElapsedSeconds = elapsedSeconds,
            };
        }
        catch (Exception ex)
        {
            var elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
            return new LoaPublishingResult
            {
                Success = false,
                ElapsedSeconds = elapsedSeconds,
                ErrorMessage = $"Problem executing Publishing: {ex.Message}",
            };
        }
    }
}
