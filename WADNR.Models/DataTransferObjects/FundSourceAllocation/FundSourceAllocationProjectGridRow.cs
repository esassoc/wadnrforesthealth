namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationProjectGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string FhtProjectNumber { get; set; } = string.Empty;
    public string? ProjectStageName { get; set; }
    public decimal? MatchAmount { get; set; }
    public decimal? PayAmount { get; set; }
    public decimal? TotalAmount { get; set; }
}
