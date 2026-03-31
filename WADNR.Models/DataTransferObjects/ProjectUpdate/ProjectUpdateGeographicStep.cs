namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for geographic assignment steps in the Project Update workflow
/// (Priority Landscapes, DNR Upland Regions, Counties).
/// </summary>
public class ProjectUpdateGeographicStep
{
    public int ProjectUpdateBatchID { get; set; }
    public List<int> SelectedIDs { get; set; } = new();
    public string? NoSelectionExplanation { get; set; }
    /// <summary>
    /// Available options for selection (auto-populated from spatial intersection).
    /// </summary>
    public List<GeographicLookupItem> AvailableOptions { get; set; } = new();
}

/// <summary>
/// Request for saving geographic assignment steps in the Project Update workflow.
/// </summary>
public class ProjectUpdateGeographicStepRequest
{
    public List<int> SelectedIDs { get; set; } = new();
    public string? NoSelectionExplanation { get; set; }
}
