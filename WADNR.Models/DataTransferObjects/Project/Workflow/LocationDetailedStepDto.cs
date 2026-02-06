namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Location Detailed step of the Project Create workflow.
/// </summary>
public class LocationDetailedStep
{
    public int ProjectID { get; set; }
    public List<ProjectLocationItem> Locations { get; set; } = new();
    public List<ProjectLocationStagingItem> StagedLocations { get; set; } = new();
}

/// <summary>
/// A project location polygon/geometry.
/// </summary>
public class ProjectLocationItem
{
    public int ProjectLocationID { get; set; }
    public int ProjectID { get; set; }
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
/// A staged project location from GIS upload (pending approval).
/// </summary>
public class ProjectLocationStagingItem
{
    public int ProjectLocationStagingID { get; set; }
    public int ProjectID { get; set; }
    public int ProjectLocationTypeID { get; set; }
    public string ProjectLocationTypeName { get; set; } = string.Empty;
    public string? ProjectLocationNotes { get; set; }
    public string? ProjectLocationName { get; set; }
    /// <summary>
    /// GeoJSON representation of the geometry.
    /// </summary>
    public string? GeoJson { get; set; }
    public double? AreaInAcres { get; set; }
    public bool ToBeDeleted { get; set; }
}

/// <summary>
/// Request for saving the Location Detailed step.
/// </summary>
public class LocationDetailedStepRequest
{
    public List<LocationDetailedItemRequest> Locations { get; set; } = new();
}

/// <summary>
/// Request item for a single location in the detailed step.
/// </summary>
public class LocationDetailedItemRequest
{
    public int? ProjectLocationID { get; set; }
    public int ProjectLocationTypeID { get; set; }
    public string? ProjectLocationNotes { get; set; }
    public string? ProjectLocationName { get; set; }
    /// <summary>
    /// GeoJSON representation of the geometry.
    /// </summary>
    public string GeoJson { get; set; } = string.Empty;
}
