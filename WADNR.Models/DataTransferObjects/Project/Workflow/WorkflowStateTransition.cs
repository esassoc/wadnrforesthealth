namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Request for workflow state transitions (submit, approve, return, reject, withdraw).
/// </summary>
public class WorkflowStateTransitionRequest
{
    /// <summary>
    /// Optional comment for the state transition.
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// Response for workflow state transitions.
/// </summary>
public class WorkflowStateTransitionResponse
{
    public int ProjectID { get; set; }
    public int NewProjectApprovalStatusID { get; set; }
    public string NewProjectApprovalStatusName { get; set; } = string.Empty;
    public DateTimeOffset TransitionDate { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
