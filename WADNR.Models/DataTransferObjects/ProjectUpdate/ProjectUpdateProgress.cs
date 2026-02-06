namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for Update workflow progress, including section completion states.
/// </summary>
public class UpdateWorkflowProgressResponse
{
    public int ProjectUpdateBatchID { get; set; }
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int ProjectUpdateStateID { get; set; }
    public string ProjectUpdateStateName { get; set; } = string.Empty;
    public DateTime LastUpdateDate { get; set; }
    public string? LastUpdatedByPersonName { get; set; }

    /// <summary>
    /// Person who submitted this update batch (if submitted).
    /// </summary>
    public string? SubmittedByPersonName { get; set; }

    /// <summary>
    /// Date when this update batch was submitted (if submitted).
    /// </summary>
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Person who returned this update batch for revisions (if returned).
    /// </summary>
    public string? ReturnedByPersonName { get; set; }

    /// <summary>
    /// Date when this update batch was returned (if returned).
    /// </summary>
    public DateTime? ReturnedDate { get; set; }

    public bool CanSubmit { get; set; }

    /// <summary>
    /// Whether the update batch passes all validation rules and is ready for approval.
    /// </summary>
    public bool IsReadyToApprove { get; set; }

    /// <summary>
    /// Step completion states keyed by step enum name.
    /// </summary>
    public Dictionary<string, WorkflowStepStatus> Steps { get; set; } = new();

    /// <summary>
    /// Per-section reviewer comments keyed by step enum name, populated when batch is in Returned state.
    /// </summary>
    public Dictionary<string, string?>? ReviewerComments { get; set; }

    // User permission flags
    public bool CanEdit { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReturn { get; set; }
    public bool CanDelete { get; set; }
}
