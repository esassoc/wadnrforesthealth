using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WADNR.EFModels.Entities;

namespace WADNR.API.Services;

public class FinanceApiDownloadService(
    IHttpClientFactory httpClientFactory,
    IOptions<WADNRConfiguration> configuration,
    WADNRDbContext dbContext)
{
    private const int PageSize = 1000;
    private const int StaleEntriesDayCutoff = 5;

    private readonly WADNRConfiguration _configuration = configuration.Value;

    /// <summary>
    /// Query the Finance API's last-load-date endpoint. Returns the most recent LOAD_COMPLETE_DATE.
    /// </summary>
    public async Task<DateTime> GetLastLoadDateAsync(string token)
    {
        using var httpClient = httpClientFactory.CreateClient("FinanceApi");
        var queryUrl =
            $"{_configuration.LastLoadDateUrl}?token={token}" +
            $"&f=json" +
            $"&where=1=1" +
            $"&outFields=LOAD_FREQUENCY,FINANCIAL_LOAD_HISTORY_ID,LOAD_COMPLETE_DATE" +
            $"&orderByFields=LOAD_COMPLETE_DATE DESC" +
            $"&resultRecordCount=1";

        var json = await httpClient.GetStringAsync(queryUrl);
        using var doc = JsonDocument.Parse(json);

        var features = doc.RootElement.GetProperty("features");
        if (features.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("Could not get the LastLoadDate of the finance API.");
        }

        var loadCompleteDateMs = features[0].GetProperty("attributes").GetProperty("LOAD_COMPLETE_DATE").GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(loadCompleteDateMs).DateTime;
    }

    /// <summary>
    /// Downloads paginated JSON data from an ArcGIS REST API endpoint.
    /// Returns a JSON array string of all attribute objects.
    /// </summary>
    public async Task<string> DownloadPaginatedJsonAsync(string baseUrl, string token, string whereClause,
        string outFields, string orderByFields)
    {
        using var httpClient = httpClientFactory.CreateClient("FinanceApi");
        var results = new List<string>();
        var offset = 0;
        var hasMoreData = true;

        while (hasMoreData)
        {
            var queryUrl =
                $"{baseUrl}?token={token}" +
                $"&f=json" +
                $"&where={whereClause}" +
                $"&outFields={outFields}" +
                (string.IsNullOrEmpty(orderByFields) ? "" : $"&orderByFields={orderByFields}") +
                $"&resultRecordCount={PageSize}" +
                $"&resultOffset={offset}";

            string responseText;
            try
            {
                responseText = await httpClient.GetStringAsync(queryUrl);
            }
            catch
            {
                // One retry on failure, matching legacy behavior
                responseText = await httpClient.GetStringAsync(queryUrl);
            }

            using var doc = JsonDocument.Parse(responseText);
            var root = doc.RootElement;

            hasMoreData = root.TryGetProperty("exceededTransferLimit", out var exceeded) && exceeded.GetBoolean();

            foreach (var feature in root.GetProperty("features").EnumerateArray())
            {
                results.Add(feature.GetProperty("attributes").GetRawText());
                offset++;
            }
        }

        return $"[{string.Join(",", results)}]";
    }

    /// <summary>
    /// Stores raw JSON into the ArcOnlineFinanceApiRawJsonImport staging table.
    /// Returns the new import ID.
    /// </summary>
    public async Task<int> StoreRawJsonImportAsync(int tableTypeID, DateTime lastLoadDate, int? bienniumFiscalYear,
        string rawJsonString)
    {
        var import = new ArcOnlineFinanceApiRawJsonImport
        {
            CreateDate = DateTime.Now,
            ArcOnlineFinanceApiRawJsonImportTableTypeID = tableTypeID,
            FinanceApiLastLoadDate = lastLoadDate,
            BienniumFiscalYear = bienniumFiscalYear,
            RawJsonString = rawJsonString,
            JsonImportStatusTypeID = (int)JsonImportStatusTypeEnum.NotYetProcessed
        };

        dbContext.ArcOnlineFinanceApiRawJsonImports.Add(import);
        await dbContext.SaveChangesWithNoAuditingAsync();

        return import.ArcOnlineFinanceApiRawJsonImportID;
    }

    /// <summary>
    /// Gets the latest successful import info for a given table type and optional biennium.
    /// Returns null if no successful import exists.
    /// </summary>
    public async Task<SuccessfulJsonImportInfo?> GetLatestSuccessfulImportAsync(int tableTypeID,
        int? bienniumFiscalYear)
    {
        var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();
        try
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableTypeFromArcOnlineFinanceApi";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            var tableTypeParam = cmd.CreateParameter();
            tableTypeParam.ParameterName = "@ArcOnlineFinanceApiRawJsonImportTableTypeID";
            tableTypeParam.Value = tableTypeID;
            cmd.Parameters.Add(tableTypeParam);

            var bienniumParam = cmd.CreateParameter();
            bienniumParam.ParameterName = "@OptionalBienniumFiscalYear";
            bienniumParam.Value = bienniumFiscalYear.HasValue ? bienniumFiscalYear.Value : DBNull.Value;
            cmd.Parameters.Add(bienniumParam);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new SuccessfulJsonImportInfo
            {
                ArcOnlineFinanceApiRawJsonImportTableTypeID =
                    (int)reader["ArcOnlineFinanceApiRawJsonImportTableTypeID"],
                BienniumFiscalYear = reader["BienniumFiscalYear"] != DBNull.Value
                    ? (int?)reader["BienniumFiscalYear"]
                    : null,
                JsonImportDate = (DateTime)reader["JsonImportDate"],
                FinanceApiLastLoadDate = (DateTime)reader["FinanceApiLastLoadDate"]
            };
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    /// <summary>
    /// Marks an import record's status via the stored procedure.
    /// </summary>
    public async Task MarkImportStatusAsync(int importID, JsonImportStatusTypeEnum status)
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.pArcOnlineMarkJsonImportStatus @ArcOnlineFinanceApiRawJsonImportID = {0}, @JsonImportStatusTypeID = {1}",
            importID, (int)status);
    }

    /// <summary>
    /// Clears import records older than StaleEntriesDayCutoff days.
    /// </summary>
    public async Task ClearOutdatedImportsAsync()
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.pClearOutdatedArcOnlineFinanceApiRawJsonImports @daysOldToRemove = {0}",
            StaleEntriesDayCutoff);
    }

    /// <summary>
    /// Executes an entity-specific import stored procedure.
    /// </summary>
    public async Task ExecuteImportProcAsync(string procName, int importID, int? biennium = null)
    {
        if (biennium.HasValue)
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                $"EXEC {procName} @ArcOnlineFinanceApiRawJsonImportID = {{0}}, @BienniumToImport = {{1}}",
                importID, biennium.Value);
        }
        else
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                $"EXEC {procName} @ArcOnlineFinanceApiRawJsonImportID = {{0}}",
                importID);
        }
    }

    /// <summary>
    /// Clears fund source allocation expenditure tables for a given biennium.
    /// </summary>
    public async Task ClearFundSourceAllocationExpenditureTablesAsync(int bienniumFiscalYear)
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.pClearFundSourceAllocationExpenditureTables @bienniumFiscalYear = {0}",
            bienniumFiscalYear);
    }

    /// <summary>
    /// Gets the current biennium fiscal year from the database.
    /// </summary>
    public async Task<int> GetCurrentBienniumFiscalYearAsync()
    {
        var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();
        try
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT dbo.fGetFiscalYearBienniumForDate(GETDATE())";
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public class SuccessfulJsonImportInfo
    {
        public int ArcOnlineFinanceApiRawJsonImportTableTypeID { get; set; }
        public int? BienniumFiscalYear { get; set; }
        public DateTime JsonImportDate { get; set; }
        public DateTime FinanceApiLastLoadDate { get; set; }
    }
}
