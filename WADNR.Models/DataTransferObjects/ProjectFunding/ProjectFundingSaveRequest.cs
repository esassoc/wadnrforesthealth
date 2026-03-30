namespace WADNR.Models.DataTransferObjects;

public class ProjectFundingSaveRequest
{
    public decimal? EstimatedTotalCost { get; set; }
    public string? FundingSourceNotes { get; set; }
    public List<int> FundingSourceIDs { get; set; } = new();
    public List<ProjectFundSourceAllocationRequestItemRequest> AllocationRequests { get; set; } = new();
}

public class ProjectFundSourceAllocationRequestItemRequest
{
    public int? ProjectFundSourceAllocationRequestID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public decimal? MatchAmount { get; set; }
    public decimal? PayAmount { get; set; }
    public decimal? TotalAmount { get; set; }
}
