namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for a single step's diff check in the Project Update workflow.
/// </summary>
public class StepDiffResponse
{
    /// <summary>
    /// Whether this step has changes compared to the approved project.
    /// </summary>
    public bool HasChanges { get; set; }

    /// <summary>
    /// HTML diff showing additions (green) and deletions (red).
    /// Only populated if HasChanges is true.
    /// </summary>
    public string? DiffHtml { get; set; }
}
