using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("Treatment")]
public partial class Treatment
{
    [Key]
    public int TreatmentID { get; set; }

    public int ProjectID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TreatmentStartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TreatmentEndDate { get; set; }

    [Column(TypeName = "decimal(38, 10)")]
    public decimal TreatmentFootprintAcres { get; set; }

    [StringLength(2000)]
    [Unicode(false)]
    public string? TreatmentNotes { get; set; }

    public int TreatmentTypeID { get; set; }

    [Column(TypeName = "decimal(38, 10)")]
    public decimal? TreatmentTreatedAcres { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? TreatmentTypeImportedText { get; set; }

    public int? CreateGisUploadAttemptID { get; set; }

    public int? UpdateGisUploadAttemptID { get; set; }

    public int TreatmentDetailedActivityTypeID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? TreatmentDetailedActivityTypeImportedText { get; set; }

    public int? ProgramID { get; set; }

    public bool? ImportedFromGis { get; set; }

    public int? ProjectLocationID { get; set; }

    public int? TreatmentCodeID { get; set; }

    [Column(TypeName = "money")]
    public decimal? CostPerAcre { get; set; }
}
