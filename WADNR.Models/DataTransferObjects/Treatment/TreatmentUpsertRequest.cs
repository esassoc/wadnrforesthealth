using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class TreatmentUpsertRequest
{
    [Required]
    public int ProjectID { get; set; }

    [Required]
    public int ProjectLocationID { get; set; }

    [Required]
    public int TreatmentTypeID { get; set; }

    [Required]
    public int TreatmentDetailedActivityTypeID { get; set; }

    public int? TreatmentCodeID { get; set; }

    [Required]
    public DateTime TreatmentStartDate { get; set; }

    [Required]
    public DateTime TreatmentEndDate { get; set; }

    [Required]
    public decimal TreatmentFootprintAcres { get; set; }

    public decimal? TreatmentTreatedAcres { get; set; }

    public decimal? CostPerAcre { get; set; }

    [StringLength(2000)]
    public string? TreatmentNotes { get; set; }

    public int? ProgramID { get; set; }
}
