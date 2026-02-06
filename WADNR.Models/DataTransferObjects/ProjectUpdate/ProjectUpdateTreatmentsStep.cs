using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Treatments step of the Project Update workflow.
/// </summary>
public class ProjectUpdateTreatmentsStep
{
    public int ProjectUpdateBatchID { get; set; }
    public List<TreatmentUpdateItem> Treatments { get; set; } = new();
}

/// <summary>
/// A treatment in an Update batch.
/// </summary>
public class TreatmentUpdateItem
{
    public int TreatmentUpdateID { get; set; }
    public int ProjectUpdateBatchID { get; set; }
    public int? ProjectLocationUpdateID { get; set; }
    public string? ProjectLocationName { get; set; }
    public int TreatmentTypeID { get; set; }
    public string TreatmentTypeName { get; set; } = string.Empty;
    public int TreatmentDetailedActivityTypeID { get; set; }
    public string TreatmentDetailedActivityTypeName { get; set; } = string.Empty;
    public int? TreatmentCodeID { get; set; }
    public string? TreatmentCodeName { get; set; }
    public decimal? TreatmentFootprintAcres { get; set; }
    public decimal? TreatmentTreatedAcres { get; set; }
    public string? TreatmentNotes { get; set; }
    public int? TreatmentStartYear { get; set; }
    public int? TreatmentEndYear { get; set; }
    public decimal? CostPerAcre { get; set; }
    public decimal? TotalCost { get; set; }
    public bool ImportedFromGis { get; set; }
}

/// <summary>
/// Request for saving the Treatments step of the Project Update workflow.
/// </summary>
public class ProjectUpdateTreatmentsStepRequest
{
    public List<TreatmentUpdateItemRequest> Treatments { get; set; } = new();
}

/// <summary>
/// Request item for a single treatment in the Update Treatments step.
/// </summary>
public class TreatmentUpdateItemRequest
{
    public int? TreatmentUpdateID { get; set; }
    public int? ProjectLocationUpdateID { get; set; }
    public int TreatmentTypeID { get; set; }
    public int TreatmentDetailedActivityTypeID { get; set; }
    public int? TreatmentCodeID { get; set; }
    public decimal? TreatmentFootprintAcres { get; set; }
    public decimal? TreatmentTreatedAcres { get; set; }
    public string? TreatmentNotes { get; set; }
    public int? TreatmentStartYear { get; set; }
    public int? TreatmentEndYear { get; set; }
    public decimal? CostPerAcre { get; set; }
    public decimal? TotalCost { get; set; }
}

/// <summary>
/// Detailed view of a single TreatmentUpdate for edit modal pre-fill.
/// </summary>
public class TreatmentUpdateDetail
{
    public int TreatmentUpdateID { get; set; }
    public int ProjectUpdateBatchID { get; set; }
    public int? ProjectLocationUpdateID { get; set; }
    public string? TreatmentAreaName { get; set; }
    public int TreatmentTypeID { get; set; }
    public string TreatmentTypeName { get; set; } = string.Empty;
    public int TreatmentDetailedActivityTypeID { get; set; }
    public string TreatmentDetailedActivityTypeName { get; set; } = string.Empty;
    public int? TreatmentCodeID { get; set; }
    public string? TreatmentCodeName { get; set; }
    public DateTime? TreatmentStartDate { get; set; }
    public DateTime? TreatmentEndDate { get; set; }
    public decimal TreatmentFootprintAcres { get; set; }
    public decimal? TreatmentTreatedAcres { get; set; }
    public decimal? CostPerAcre { get; set; }
    public decimal? TotalCost { get; set; }
    public string? TreatmentNotes { get; set; }
    public int? ProgramID { get; set; }
    public string? ProgramName { get; set; }
    public bool ImportedFromGis { get; set; }
}

/// <summary>
/// Request for creating or updating a single TreatmentUpdate record.
/// </summary>
public class TreatmentUpdateUpsertRequest
{
    [Required] public int ProjectLocationUpdateID { get; set; }
    [Required] public int TreatmentTypeID { get; set; }
    [Required] public int TreatmentDetailedActivityTypeID { get; set; }
    public int? TreatmentCodeID { get; set; }
    [Required] public DateTime TreatmentStartDate { get; set; }
    [Required] public DateTime TreatmentEndDate { get; set; }
    [Required] public decimal TreatmentFootprintAcres { get; set; }
    public decimal? TreatmentTreatedAcres { get; set; }
    public decimal? CostPerAcre { get; set; }
    [StringLength(2000)] public string? TreatmentNotes { get; set; }
    public int? ProgramID { get; set; }
}

/// <summary>
/// Lookup item for treatment area dropdowns in the update workflow.
/// </summary>
public class TreatmentAreaUpdateLookupItem
{
    public int ProjectLocationUpdateID { get; set; }
    public string ProjectLocationUpdateName { get; set; } = string.Empty;
}
