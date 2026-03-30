using ClosedXML.Excel;

namespace WADNR.Common.ExcelWorkbookUtilities;

public class LoaStageRow
{
    public string ProjectID { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateTime? LetterDate { get; set; }
    public DateTime? ProjectExpirationDate { get; set; }
    public DateTime? ApplicationDate { get; set; }
    public DateTime? DecisionDate { get; set; }
    public string? FundSourceNumber { get; set; }
    public string? FocusArea { get; set; }
    public string? ProjectCode { get; set; }
    public string? ProgramIndex { get; set; }
    public double? MatchAmount { get; set; }
    public double? PayAmount { get; set; }
    public string? Forester { get; set; }
    public string? ForesterPhone { get; set; }
    public string? ForesterEmail { get; set; }
}

public static class LoaExcelParser
{
    private const string MasterDataSheetName = "Master Data";

    // Expected column names in the header row
    private static readonly string[] ExpectedColumns =
    [
        "Project ID",
        "Status",
        "Letter Date",
        "Project Expiration Date",
        "Application Date",
        "Decision Date",
        "FundSource #",
        "FundSource",
        "Code",
        "Index",
        "Match",
        "Pay",
        "Forester",
        "Forester Phone",
        "Forester email"
    ];

    public static List<LoaStageRow> ParseExcelFile(Stream stream, List<string> errorList)
    {
        using var workbook = new XLWorkbook(stream);

        // Find the worksheet: prefer "Master Data", fallback to single sheet
        IXLWorksheet worksheet;
        if (workbook.Worksheets.TryGetWorksheet(MasterDataSheetName, out var masterSheet))
        {
            worksheet = masterSheet;
        }
        else if (workbook.Worksheets.Count == 1)
        {
            worksheet = workbook.Worksheets.First();
        }
        else
        {
            throw new InvalidOperationException(
                $"Could not find worksheet \"{MasterDataSheetName}\" and the workbook has multiple sheets. " +
                "Please ensure the Excel file has a \"Master Data\" sheet.");
        }

        // Detect header row: some files have a column-letter row above the headers (Row 1 = letters,
        // Row 2 = human-readable names), others have headers directly in Row 1.
        var (columnMapping, headerRowNum) = BuildColumnMapping(worksheet);
        var firstDataRow = headerRowNum + 1;
        var rows = new List<LoaStageRow>();

        var lastRowUsed = worksheet.LastRowUsed()?.RowNumber() ?? firstDataRow;
        for (var rowNum = firstDataRow; rowNum <= lastRowUsed; rowNum++)
        {
            var row = worksheet.Row(rowNum);
            if (IsRowBlank(row, columnMapping))
            {
                continue;
            }

            var projectID = GetStringValue(row, columnMapping, "Project ID");
            if (string.IsNullOrWhiteSpace(projectID))
            {
                continue;
            }

            var stageRow = new LoaStageRow
            {
                ProjectID = projectID,
                Status = GetStringValue(row, columnMapping, "Status"),
                FundSourceNumber = GetStringValue(row, columnMapping, "FundSource #"),
                FocusArea = GetStringValue(row, columnMapping, "FundSource"),
                ProjectCode = GetStringValue(row, columnMapping, "Code"),
                ProgramIndex = GetStringValue(row, columnMapping, "Index"),
                Forester = GetStringValue(row, columnMapping, "Forester"),
                ForesterPhone = GetStringValue(row, columnMapping, "Forester Phone"),
                ForesterEmail = GetStringValue(row, columnMapping, "Forester email"),
            };

            stageRow.LetterDate = GetDateValue(row, rowNum, columnMapping, "Letter Date", errorList);
            stageRow.ProjectExpirationDate = GetDateValue(row, rowNum, columnMapping, "Project Expiration Date", errorList);
            stageRow.ApplicationDate = GetDateValue(row, rowNum, columnMapping, "Application Date", errorList);
            stageRow.DecisionDate = GetDateValue(row, rowNum, columnMapping, "Decision Date", errorList);
            stageRow.MatchAmount = GetDoubleValue(row, columnMapping, "Match");
            stageRow.PayAmount = GetDoubleValue(row, columnMapping, "Pay");

            rows.Add(stageRow);
        }

        return rows;
    }

    private static (Dictionary<string, int> mapping, int headerRowNum) BuildColumnMapping(IXLWorksheet worksheet)
    {
        var lastColUsed = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

        // Try Row 1 first (headers directly in first row), then Row 2 (legacy format
        // where Row 1 has column-letter identifiers and Row 2 has human-readable names)
        foreach (var candidateRow in new[] { 1, 2 })
        {
            var row = worksheet.Row(candidateRow);
            var mapping = new Dictionary<string, int>();

            for (var col = 1; col <= lastColUsed; col++)
            {
                var cellValue = row.Cell(col).GetString().Trim();
                if (!string.IsNullOrEmpty(cellValue))
                {
                    mapping[cellValue] = col;
                }
            }

            var missingColumns = ExpectedColumns.Where(c => !mapping.ContainsKey(c)).ToList();
            if (missingColumns.Count == 0)
            {
                return (mapping, candidateRow);
            }
        }

        // Neither row matched — build error from Row 1 for the error message
        var row1Mapping = new Dictionary<string, int>();
        for (var col = 1; col <= lastColUsed; col++)
        {
            var cellValue = worksheet.Row(1).Cell(col).GetString().Trim();
            if (!string.IsNullOrEmpty(cellValue))
            {
                row1Mapping[cellValue] = col;
            }
        }

        var missing = ExpectedColumns.Where(c => !row1Mapping.ContainsKey(c)).ToList();
        var actual = row1Mapping.Keys.OrderBy(k => row1Mapping[k]).ToList();
        throw new InvalidOperationException(
            $"Expected columns [{string.Join(", ", ExpectedColumns)}]\n\n" +
            $"But got columns [{string.Join(", ", actual)}].\n\n" +
            $"These columns were missing: [{string.Join(", ", missing)}]");
    }

    private static bool IsRowBlank(IXLRow row, Dictionary<string, int> columnMapping)
    {
        return columnMapping.Values.All(colIndex =>
            string.IsNullOrWhiteSpace(row.Cell(colIndex).GetString()));
    }

    private static string? GetStringValue(IXLRow row, Dictionary<string, int> columnMapping, string columnName)
    {
        var colIndex = columnMapping[columnName];
        var value = row.Cell(colIndex).GetString().Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static double? GetDoubleValue(IXLRow row, Dictionary<string, int> columnMapping, string columnName)
    {
        var colIndex = columnMapping[columnName];
        var cellValue = row.Cell(colIndex).GetString().Trim();
        if (string.IsNullOrWhiteSpace(cellValue))
        {
            return null;
        }

        return double.TryParse(cellValue, out var result) ? result : null;
    }

    private static DateTime? GetDateValue(IXLRow row, int rowNum, Dictionary<string, int> columnMapping,
        string columnName, List<string> errorList)
    {
        var colIndex = columnMapping[columnName];
        var cell = row.Cell(colIndex);
        var cellValue = cell.GetString().Trim();

        // "#" means null in the legacy format
        if (cellValue == "#" || string.IsNullOrWhiteSpace(cellValue))
        {
            return null;
        }

        // Try OLE Automation date (serial number like 39938 = 05/05/2009)
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

        // Try standard DateTime parsing
        if (DateTime.TryParse(cellValue, out var parsedDate))
        {
            return parsedDate;
        }

        // Legacy comma-fix fallback: "January 1,2020" → "January 1, 2020"
        var updatedCellValue = cellValue.Replace(",", ", ");
        if (DateTime.TryParse(updatedCellValue, out var fixedDate))
        {
            return fixedDate;
        }

        errorList.Add($"Row {rowNum}, Column \"{columnName}\": Could not parse date value \"{cellValue}\"");
        return null;
    }
}
