namespace WADNR.Models.DataTransferObjects;

public class TreatmentGridRow
{
    public int TreatmentID { get; set; }
    public string? TreatmentAreaName { get; set; }
    public string TreatmentTypeName { get; set; } = string.Empty;
    public string TreatmentDetailedActivityTypeName { get; set; } = string.Empty;
    public DateTime? TreatmentStartDate { get; set; }
    public DateTime? TreatmentEndDate { get; set; }
    public decimal TreatmentFootprintAcres { get; set; }
    public decimal? TreatmentTreatedAcres { get; set; }
    public decimal? CostPerAcre { get; set; }
    public decimal? TotalCost { get; set; }
    public string? TreatmentNotes { get; set; }
    public string? ProgramName { get; set; }
    public string? TreatmentCodeName { get; set; }
    public bool ImportedFromGis { get; set; }
}
