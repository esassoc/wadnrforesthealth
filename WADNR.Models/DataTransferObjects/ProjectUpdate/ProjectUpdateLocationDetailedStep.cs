namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Location Detailed step of the Project Update workflow.
/// </summary>
public class ProjectUpdateLocationDetailedStep
{
    public int ProjectUpdateBatchID { get; set; }
    public List<ProjectLocationUpdateItem> Locations { get; set; } = new();
}

/// <summary>
/// A project location polygon/geometry in an Update batch.
/// </summary>
public class ProjectLocationUpdateItem
{
    public int ProjectLocationUpdateID { get; set; }
    public int ProjectUpdateBatchID { get; set; }
    public int ProjectLocationTypeID { get; set; }
    public string ProjectLocationTypeName { get; set; } = string.Empty;
    public string? ProjectLocationNotes { get; set; }
    public string? ProjectLocationName { get; set; }
    /// <summary>
    /// GeoJSON representation of the geometry.
    /// </summary>
    public string? GeoJson { get; set; }
    public double? AreaInAcres { get; set; }
    public bool HasTreatments { get; set; }
    public bool IsFromArcGis { get; set; }
}

/// <summary>
/// Request for saving the Location Detailed step of the Project Update workflow.
/// </summary>
public class ProjectUpdateLocationDetailedStepRequest
{
    public List<ProjectLocationUpdateItemRequest> Locations { get; set; } = new();
}

/// <summary>
/// Request item for a single location in the Update Location Detailed step.
/// </summary>
public class ProjectLocationUpdateItemRequest
{
    public int? ProjectLocationUpdateID { get; set; }
    public int ProjectLocationTypeID { get; set; }
    public string? ProjectLocationNotes { get; set; }
    public string? ProjectLocationName { get; set; }
    /// <summary>
    /// GeoJSON representation of the geometry.
    /// </summary>
    public string GeoJson { get; set; } = string.Empty;
}
