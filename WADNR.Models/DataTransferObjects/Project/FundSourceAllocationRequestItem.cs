namespace WADNR.Models.DataTransferObjects;

public class FundSourceAllocationRequestItem
{
    public int ProjectFundSourceAllocationRequestID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public string FundSourceAllocationName { get; set; } = string.Empty;
    public string FundSourceName { get; set; } = string.Empty;
    public decimal? MatchAmount { get; set; }
    public decimal? PayAmount { get; set; }
    public decimal? TotalAmount { get; set; }
}
