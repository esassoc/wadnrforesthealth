namespace WADNR.Models.DataTransferObjects;

public class TreatmentDetail
{
    public int TreatmentID { get; set; }
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int? ProjectLocationID { get; set; }
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
