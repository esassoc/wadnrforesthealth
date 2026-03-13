namespace WADNR.Models.DataTransferObjects;

public class FundSourceGridRow
{
    public int FundSourceID { get; set; }
    public string FundSourceName { get; set; } = string.Empty;
    public string? FundSourceNumber { get; set; }
    public string? ShortName { get; set; }
    public decimal TotalAwardAmount { get; set; }
    public decimal CurrentBalance { get; set; }
    public string? CFDANumber { get; set; }
    public string FundSourceTitle { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string FundSourceStatusName { get; set; } = string.Empty;
    public string? FundSourceTypeDisplay { get; set; }
}
