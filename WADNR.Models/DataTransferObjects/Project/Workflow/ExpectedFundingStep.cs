namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Expected Funding step of the Project Create workflow.
/// Note: Available funding sources should be fetched from GET /lookups/funding-sources
/// Note: Available fund source allocations should be fetched from GET /fund-source-allocations/lookup
/// </summary>
public class ExpectedFundingStep
{
    public int ProjectID { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public string? ProjectFundingSourceNotes { get; set; }
    /// <summary>
    /// Selected funding source IDs (Federal, State, Private, Other checkboxes).
    /// </summary>
    public List<int> SelectedFundingSourceIDs { get; set; } = new();
    /// <summary>
    /// Fund source allocation requests for this project.
    /// </summary>
    public List<FundSourceAllocationRequestStepItem> AllocationRequests { get; set; } = new();
}

/// <summary>
/// A fund source allocation request for a project.
/// </summary>
public class FundSourceAllocationRequestStepItem
{
    public int? ProjectFundSourceAllocationRequestID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public string FundSourceAllocationName { get; set; } = string.Empty;
    public string FundSourceName { get; set; } = string.Empty;
    public decimal? MatchAmount { get; set; }
    public decimal? PayAmount { get; set; }
    public decimal? TotalAmount { get; set; }
}

/// <summary>
/// Request for saving the Expected Funding step.
/// </summary>
public class ExpectedFundingStepRequest
{
    public decimal? EstimatedTotalCost { get; set; }
    public string? ProjectFundingSourceNotes { get; set; }
    public List<int> FundingSourceIDs { get; set; } = new();
    public List<FundSourceAllocationRequestRequestItem> AllocationRequests { get; set; } = new();
}

/// <summary>
/// Request item for a fund source allocation request.
/// </summary>
public class FundSourceAllocationRequestRequestItem
{
    public int? ProjectFundSourceAllocationRequestID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public decimal? TotalAmount { get; set; }
}
