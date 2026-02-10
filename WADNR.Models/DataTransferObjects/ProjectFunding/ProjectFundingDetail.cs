namespace WADNR.Models.DataTransferObjects;

public class ProjectFundingDetail
{
    public decimal? EstimatedTotalCost { get; set; }
    public string? FundingSourceNotes { get; set; }
    public List<int> SelectedFundingSourceIDs { get; set; } = new();
    public List<FundSourceAllocationRequestItem> AllocationRequests { get; set; } = new();
}
