namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Status of a single workflow step (used by both Create and Update workflows).
/// </summary>
public class WorkflowStepStatus
{
    public bool IsComplete { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsRequired { get; set; }
    /// <summary>
    /// For Update workflow only: indicates whether this step has changes compared to the approved project.
    /// </summary>
    public bool HasChanges { get; set; }
}
