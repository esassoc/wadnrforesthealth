namespace WADNR.Models.DataTransferObjects;

public class FundSourceExcelRow
{
    public string? FundSourceNumber { get; set; }
    public string? CFDANumber { get; set; }
    public string FundSourceName { get; set; } = string.Empty;
    public decimal TotalAwardAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string FundSourceStatusName { get; set; } = string.Empty;
    public string? FundSourceTypeDisplay { get; set; }
}
