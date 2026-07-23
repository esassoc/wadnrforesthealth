using ClosedXML.Excel;

namespace WADNR.Common.ExcelWorkbookUtilities;

public class ServiceForestryStageRow
{
    public string? RegionTitle { get; set; }
    public string ProjectIdentifier { get; set; } = string.Empty;
    public DateTime? ApprovalDate { get; set; }
    public string? County { get; set; }
    public string? Forester { get; set; }
    public decimal? TotalAcres { get; set; }
    public bool? StewardshipPlan { get; set; }
    public decimal? PercentMatch { get; set; }
    public string? FundSource { get; set; }
    public string? DCStatus { get; set; }
    public decimal? DCAllocatedAmount { get; set; }
    public DateTime? DCLetterDate { get; set; }
    public DateTime? DCExpirationDate { get; set; }

    public string?[] DCTreatments { get; } = new string?[6];
    public decimal?[] DCCosts { get; } = new decimal?[6];
    public decimal?[] DCCostPerAcres { get; } = new decimal?[6];
    public decimal?[] DCAcresTreatments { get; } = new decimal?[6];

    public decimal? DCTotalMaxAmount { get; set; }
    public decimal? DCTreatedAcres { get; set; }
    public bool? DCContractor { get; set; }
    public string? DCVendorName1 { get; set; }
    public string? DCVendorName2 { get; set; }
    public string? DCVendorAddress1 { get; set; }
    public string? DCVendorAddress2 { get; set; }
    public string? DCSwvVendorNumber { get; set; }
    public DateTime? DCInvoiceDate { get; set; }
    public string? DCProgramIndex { get; set; }
    public string? DCProjectCode { get; set; }
    public decimal? DCMatchAmount { get; set; }
    public decimal? DCPayAmount { get; set; }
    public string? ItemType { get; set; }
    public string? SourcePath { get; set; }
}

/// <summary>
/// Parses the "DNR Service Forestry ALL REGIONS" Excel export (a single "query" worksheet
/// with a header row and one row per project). Mirrors <see cref="LoaExcelParser"/> but is
/// tolerant of optional columns — only a small set of identifying columns are required.
/// </summary>
public static class ServiceForestryExcelParser
{
    private const string PreferredSheetName = "query";

    // A representative subset used to validate that the correct file/sheet was uploaded.
    // (Column names are matched after trimming, so trailing spaces in the source are fine.)
    private static readonly string[] RequiredColumns =
    [
        "PROJECT ID",
        "Forester",
        "FUND Source",
        "DC Status",
        "DC Project CODE",
    ];

    public static List<ServiceForestryStageRow> ParseExcelFile(Stream stream, List<string> errorList)
    {
        using var workbook = new XLWorkbook(stream);

        IXLWorksheet worksheet;
        if (workbook.Worksheets.TryGetWorksheet(PreferredSheetName, out var preferredSheet))
        {
            worksheet = preferredSheet;
        }
        else if (workbook.Worksheets.Count == 1)
        {
            worksheet = workbook.Worksheets.First();
        }
        else
        {
            throw new InvalidOperationException(
                $"Could not find worksheet \"{PreferredSheetName}\" and the workbook has multiple sheets. " +
                $"Please ensure the Excel file has a \"{PreferredSheetName}\" sheet.");
        }

        var (columnMapping, headerRowNum) = BuildColumnMapping(worksheet);
        var firstDataRow = headerRowNum + 1;
        var rows = new List<ServiceForestryStageRow>();

        var lastRowUsed = worksheet.LastRowUsed()?.RowNumber() ?? firstDataRow;
        for (var rowNum = firstDataRow; rowNum <= lastRowUsed; rowNum++)
        {
            var row = worksheet.Row(rowNum);
            if (IsRowBlank(row, columnMapping))
            {
                continue;
            }

            var projectID = GetString(row, columnMapping, "PROJECT ID");
            if (string.IsNullOrWhiteSpace(projectID))
            {
                continue;
            }

            var stageRow = new ServiceForestryStageRow
            {
                RegionTitle = GetString(row, columnMapping, "Title"),
                ProjectIdentifier = projectID,
                County = GetString(row, columnMapping, "County"),
                Forester = GetString(row, columnMapping, "Forester"),
                TotalAcres = GetDecimal(row, columnMapping, "Total Acres"),
                StewardshipPlan = GetBool(row, columnMapping, "Stewardship Plan"),
                PercentMatch = GetDecimal(row, columnMapping, "Percent Match"),
                FundSource = GetString(row, columnMapping, "FUND Source"),
                DCStatus = GetString(row, columnMapping, "DC Status"),
                DCAllocatedAmount = GetDecimal(row, columnMapping, "DC Allocated Amount"),
                DCTotalMaxAmount = GetDecimal(row, columnMapping, "DC Total Max Amount"),
                DCTreatedAcres = GetDecimal(row, columnMapping, "DC Treated Acres"),
                DCContractor = GetBool(row, columnMapping, "DC Contractor?"),
                DCVendorName1 = GetString(row, columnMapping, "DC Vendor Name1"),
                DCVendorName2 = GetString(row, columnMapping, "DC Vendor Name 2"),
                DCVendorAddress1 = GetString(row, columnMapping, "DC Vendor Address 1"),
                DCVendorAddress2 = GetString(row, columnMapping, "DC Vendor Address2"),
                DCSwvVendorNumber = GetString(row, columnMapping, "DC SWV Number / Vendor Number"),
                DCProgramIndex = GetString(row, columnMapping, "DC Program INDEX"),
                DCProjectCode = GetString(row, columnMapping, "DC Project CODE"),
                DCMatchAmount = GetDecimal(row, columnMapping, "DC Match Amount"),
                DCPayAmount = GetDecimal(row, columnMapping, "DC Pay Amount"),
                ItemType = GetString(row, columnMapping, "Item Type"),
                SourcePath = GetString(row, columnMapping, "Path"),
            };

            stageRow.ApprovalDate = GetDate(row, rowNum, columnMapping, "Approval Date", errorList);
            stageRow.DCLetterDate = GetDate(row, rowNum, columnMapping, "DC Letter Date", errorList);
            stageRow.DCExpirationDate = GetDate(row, rowNum, columnMapping, "DC Expiration Date", errorList);
            stageRow.DCInvoiceDate = GetDate(row, rowNum, columnMapping, "DC Invoice Date", errorList);

            // Six repeating treatment blocks. The "$/ac" header has no trailing space on #1
            // ("DC $/ac1") but a space variant exists elsewhere; both forms are tolerated.
            for (var i = 0; i < 6; i++)
            {
                var n = i + 1;
                stageRow.DCTreatments[i] = GetString(row, columnMapping, $"DC Treatment {n}");
                stageRow.DCCosts[i] = GetDecimal(row, columnMapping, $"DC Cost {n}");
                stageRow.DCCostPerAcres[i] = GetDecimal(row, columnMapping, $"DC $/ac{n}");
                stageRow.DCAcresTreatments[i] = GetDecimal(row, columnMapping, $"DC Acres Treatment {n}")
                    ?? GetDecimal(row, columnMapping, $"DC Acres Treatment{n}");
            }

            rows.Add(stageRow);
        }

        return rows;
    }

    private static (Dictionary<string, int> mapping, int headerRowNum) BuildColumnMapping(IXLWorksheet worksheet)
    {
        var lastColUsed = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

        // Headers are in row 1 for this export, but tolerate a leading row (legacy LOA files
        // sometimes put column-letter identifiers in row 1 and names in row 2).
        foreach (var candidateRow in new[] { 1, 2 })
        {
            var mapping = BuildMappingForRow(worksheet, candidateRow, lastColUsed);
            var missingColumns = RequiredColumns.Where(c => !mapping.ContainsKey(c)).ToList();
            if (missingColumns.Count == 0)
            {
                return (mapping, candidateRow);
            }
        }

        var row1Mapping = BuildMappingForRow(worksheet, 1, lastColUsed);
        var missing = RequiredColumns.Where(c => !row1Mapping.ContainsKey(c)).ToList();
        var actual = row1Mapping.Keys.OrderBy(k => row1Mapping[k]).ToList();
        throw new InvalidOperationException(
            $"This does not look like a Service Forestry export. Expected to find columns " +
            $"[{string.Join(", ", RequiredColumns)}]\n\n" +
            $"But got columns [{string.Join(", ", actual)}].\n\n" +
            $"These required columns were missing: [{string.Join(", ", missing)}]");
    }

    private static Dictionary<string, int> BuildMappingForRow(IXLWorksheet worksheet, int rowNum, int lastColUsed)
    {
        var row = worksheet.Row(rowNum);
        var mapping = new Dictionary<string, int>();
        for (var col = 1; col <= lastColUsed; col++)
        {
            var cellValue = row.Cell(col).GetString().Trim();
            if (!string.IsNullOrEmpty(cellValue) && !mapping.ContainsKey(cellValue))
            {
                mapping[cellValue] = col;
            }
        }
        return mapping;
    }

    private static bool IsRowBlank(IXLRow row, Dictionary<string, int> columnMapping)
    {
        return columnMapping.Values.All(colIndex =>
            string.IsNullOrWhiteSpace(row.Cell(colIndex).GetString()));
    }

    private static string? GetString(IXLRow row, Dictionary<string, int> columnMapping, string columnName)
    {
        if (!columnMapping.TryGetValue(columnName, out var colIndex))
        {
            return null;
        }
        var value = row.Cell(colIndex).GetString().Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static decimal? GetDecimal(IXLRow row, Dictionary<string, int> columnMapping, string columnName)
    {
        if (!columnMapping.TryGetValue(columnName, out var colIndex))
        {
            return null;
        }
        var cellValue = row.Cell(colIndex).GetString().Trim();
        if (string.IsNullOrWhiteSpace(cellValue))
        {
            return null;
        }

        // Strip currency/grouping symbols that occasionally appear in exports.
        var cleaned = cellValue.Replace("$", "").Replace(",", "").Trim();
        return decimal.TryParse(cleaned, out var result) ? result : null;
    }

    private static bool? GetBool(IXLRow row, Dictionary<string, int> columnMapping, string columnName)
    {
        if (!columnMapping.TryGetValue(columnName, out var colIndex))
        {
            return null;
        }
        var cellValue = row.Cell(colIndex).GetString().Trim();
        if (string.IsNullOrWhiteSpace(cellValue))
        {
            return null;
        }

        switch (cellValue.ToLowerInvariant())
        {
            case "1":
            case "true":
            case "yes":
            case "y":
                return true;
            case "0":
            case "false":
            case "no":
            case "n":
                return false;
            default:
                return null;
        }
    }

    private static DateTime? GetDate(IXLRow row, int rowNum, Dictionary<string, int> columnMapping,
        string columnName, List<string> errorList)
    {
        if (!columnMapping.TryGetValue(columnName, out var colIndex))
        {
            return null;
        }

        var cellValue = row.Cell(colIndex).GetString().Trim();
        if (cellValue == "#" || string.IsNullOrWhiteSpace(cellValue))
        {
            return null;
        }

        // OLE Automation serial date (e.g. 46148)
        if (double.TryParse(cellValue, out var serialDate))
        {
            try
            {
                return DateTime.FromOADate(serialDate);
            }
            catch
            {
                // Fall through to string parsing
            }
        }

        if (DateTime.TryParse(cellValue, out var parsedDate))
        {
            return parsedDate;
        }

        var updatedCellValue = cellValue.Replace(",", ", ");
        if (DateTime.TryParse(updatedCellValue, out var fixedDate))
        {
            return fixedDate;
        }

        errorList.Add($"Row {rowNum}, Column \"{columnName}\": Could not parse date value \"{cellValue}\"");
        return null;
    }
}
