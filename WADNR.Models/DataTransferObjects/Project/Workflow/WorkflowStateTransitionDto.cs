namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Request DTO for workflow state transitions (submit, approve, return, reject, withdraw).
/// </summary>
public class WorkflowStateTransitionRequestDto
{
    /// <summary>
    /// Optional comment for the state transition.
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// Response DTO for workflow state transitions.
/// </summary>
public class WorkflowStateTransitionResponseDto
{
    public int ProjectID { get; set; }
    public int NewProjectApprovalStatusID { get; set; }
    public string NewProjectApprovalStatusName { get; set; } = string.Empty;
    public DateTime TransitionDate { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
