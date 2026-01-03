namespace WADNR.Models.DataTransferObjects;

public class FundSourceGridRow
{
    public int FundSourceID { get; set; }
    public string FundSourceName { get; set; } = string.Empty;
    public string? FundSourceNumber { get; set; }
    public string? ShortName { get; set; }
    public decimal TotalAwardAmount { get; set; }
    public string? CFDANumber { get; set; }
    public string FundSourceTitle { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string FundSourceStatusName { get; set; } = string.Empty;
    public string? FundSourceTypeDisplay { get; set; }
}
