namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Expected Funding step of the Project Update workflow.
/// </summary>
public class ProjectUpdateExpectedFundingStep
{
    public int ProjectUpdateBatchID { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public string? ProjectFundingSourceNotes { get; set; }
    /// <summary>
    /// Selected funding source IDs (Federal, State, Private, Other checkboxes).
    /// </summary>
    public List<int> SelectedFundingSourceIDs { get; set; } = new();
    /// <summary>
    /// Fund source allocation requests for this update batch.
    /// </summary>
    public List<FundSourceAllocationRequestUpdateItem> AllocationRequests { get; set; } = new();
}

/// <summary>
/// A fund source allocation request for a project update.
/// </summary>
public class FundSourceAllocationRequestUpdateItem
{
    public int? ProjectFundSourceAllocationRequestUpdateID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public string FundSourceAllocationName { get; set; } = string.Empty;
    public string FundSourceName { get; set; } = string.Empty;
    public decimal? MatchAmount { get; set; }
    public decimal? PayAmount { get; set; }
    public decimal? TotalAmount { get; set; }
}

/// <summary>
/// Request for saving the Expected Funding step of the Project Update workflow.
/// </summary>
public class ProjectUpdateExpectedFundingStepRequest
{
    public decimal? EstimatedTotalCost { get; set; }
    public string? ProjectFundingSourceNotes { get; set; }
    public List<int> FundingSourceIDs { get; set; } = new();
    public List<FundSourceAllocationRequestUpdateItemRequest> AllocationRequests { get; set; } = new();
}

/// <summary>
/// Request item for a fund source allocation request in the update.
/// </summary>
public class FundSourceAllocationRequestUpdateItemRequest
{
    public int? ProjectFundSourceAllocationRequestUpdateID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public decimal? TotalAmount { get; set; }
}
