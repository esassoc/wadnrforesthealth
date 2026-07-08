using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using WADNR.Common.GeoSpatial;

namespace WADNR.Models.DataTransferObjects;

public class ProgramGdbExportData
{
    public List<ProgramGdbProjectPointDto> ProjectPoints { get; set; } = new();
    public List<ProgramGdbProjectLocationDto> ProjectLocations { get; set; } = new();
    public List<ProgramGdbTreatmentDto> Treatments { get; set; } = new();
}

public class ProgramGdbProjectPointDto : IHasGeometry
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = null!;
    public string FhtProjectNumber { get; set; } = null!;
    public string? ProjectDescription { get; set; }
    public string? ProjectStage { get; set; }
    public string? ProjectType { get; set; }
    public string? TaxonomyBranch { get; set; }
    public string? TaxonomyTrunk { get; set; }
    public string? LeadImplementer { get; set; }
    public string? Organizations { get; set; }
    public string? Classifications { get; set; }
    public string? Programs { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public string? ProjectLocationNotes { get; set; }
    // Column name is overridden at serialization time using the FieldDefinition label
    // for "ProjectIdentifier" so it matches the UI's configurable label.
    public string? ProjectGisIdentifier { get; set; }
    public string? Counties { get; set; }
    public string? Regions { get; set; }
    public string? PriorityLandscapes { get; set; }
    public string? FundingSources { get; set; }
    public string? ProjectDetailUrl { get; set; }
    // Point coordinates in WGS84 (SRID 4326): X = Longitude, Y = Latitude.
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    [JsonIgnore]
    public Geometry Geometry { get; set; } = null!;
}

public class ProgramGdbProjectLocationDto : IHasGeometry
{
    public int ProjectLocationID { get; set; }
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = null!;
    public string FhtProjectNumber { get; set; } = null!;
    public string? ProjectStage { get; set; }
    public string? ProjectType { get; set; }
    public string ProjectLocationName { get; set; } = null!;
    public string? ProjectLocationType { get; set; }
    public string? ProjectLocationNotes { get; set; }
    public int? ArcGisObjectID { get; set; }
    public string? ArcGisGlobalID { get; set; }
    // Emitted as "Yes"/"No" (not a raw bool) so the GDB attribute table reads clearly.
    public string? ImportedFromGisUpload { get; set; }
    public string? SourceProgram { get; set; }
    public int TreatmentCount { get; set; }
    public string? TreatmentTypes { get; set; }
    public decimal TotalFootprintAcres { get; set; }
    public decimal? TotalTreatedAcres { get; set; }
    public decimal? TotalCost { get; set; }
    public string? ProjectDetailUrl { get; set; }
    [JsonIgnore]
    public Geometry Geometry { get; set; } = null!;
}

public class ProgramGdbTreatmentDto : IHasGeometry
{
    public int TreatmentID { get; set; }
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = null!;
    public string FhtProjectNumber { get; set; } = null!;
    public int ProjectLocationID { get; set; }
    public string ProjectLocationName { get; set; } = null!;
    public string? TreatmentType { get; set; }
    public string? TreatmentTypeImportedText { get; set; }
    public string? TreatmentDetailedActivityType { get; set; }
    public string? TreatmentDetailedActivityTypeImportedText { get; set; }
    public string? TreatmentCode { get; set; }
    public DateOnly? TreatmentStartDate { get; set; }
    public DateOnly? TreatmentEndDate { get; set; }
    public decimal TreatmentFootprintAcres { get; set; }
    public decimal? TreatmentTreatedAcres { get; set; }
    public decimal? CostPerAcre { get; set; }
    public decimal? TotalCost { get; set; }
    // Emitted as "Yes"/"No" (not a raw bool) so the GDB attribute table matches the grid display.
    public string? ImportedFromGis { get; set; }
    public string? TreatmentNotes { get; set; }
    public string? ProjectDetailUrl { get; set; }
    [JsonIgnore]
    public Geometry Geometry { get; set; } = null!;
}
