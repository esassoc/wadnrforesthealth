namespace WADNR.Models.DataTransferObjects;

public class TreatmentGridRow
{
    public int TreatmentID { get; set; }
    public string TreatmentTypeName { get; set; } = string.Empty;
    public string TreatmentDetailedActivityTypeName { get; set; } = string.Empty;
    public DateTime? TreatmentStartDate { get; set; }
    public DateTime? TreatmentEndDate { get; set; }
    public decimal TreatmentFootprintAcres { get; set; }
    public decimal? TreatmentTreatedAcres { get; set; }
    public string? TreatmentNotes { get; set; }
    public string? ProgramName { get; set; }
    public string? TreatmentCodeName { get; set; }
}
