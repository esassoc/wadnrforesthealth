using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNR.EFModels.Entities;

[Table("ProjectUpdate")]
[Index("ProjectLocationPoint", Name = "SPATIAL_ProjectUpdate_ProjectLocationPoint")]
public partial class ProjectUpdate
{
    [Key]
    public int ProjectUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int ProjectStageID { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? ProjectDescription { get; set; }

    public DateOnly? CompletionDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? EstimatedTotalCost { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? ProjectLocationPoint { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? ProjectLocationNotes { get; set; }

    public DateOnly? PlannedDate { get; set; }

    public int ProjectLocationSimpleTypeID { get; set; }

    public int? FocusAreaID { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? ProjectFundingSourceNotes { get; set; }

    public int? PercentageMatch { get; set; }

    [ForeignKey("FocusAreaID")]
    [InverseProperty("ProjectUpdates")]
    public virtual FocusArea? FocusArea { get; set; }

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
