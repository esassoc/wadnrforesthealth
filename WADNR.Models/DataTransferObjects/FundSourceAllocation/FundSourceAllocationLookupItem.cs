namespace WADNR.Models.DataTransferObjects;

public class FundSourceAllocationLookupItem
{
    public int FundSourceAllocationID { get; set; }
    public string FundSourceAllocationName { get; set; } = string.Empty;
    public string FundSourceName { get; set; } = string.Empty;
}
