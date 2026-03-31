namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for geographic assignment steps (Priority Landscapes, DNR Upland Regions, Counties).
/// </summary>
public class GeographicAssignmentStep
{
    public int ProjectID { get; set; }
    public List<int> SelectedIDs { get; set; } = new();
    public string? NoSelectionExplanation { get; set; }
    /// <summary>
    /// Available options for selection (auto-populated from spatial intersection).
    /// </summary>
    public List<GeographicLookupItem> AvailableOptions { get; set; } = new();
}

/// <summary>
/// Lookup item for geographic regions.
/// </summary>
public class GeographicLookupItem
{
    public int ID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// Request for saving geographic assignment steps.
/// </summary>
public class GeographicOverrideRequest
{
    public List<int> SelectedIDs { get; set; } = new();
    public string? NoSelectionExplanation { get; set; }
}
