using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FocusArea")]
[Index("FocusAreaName", Name = "AK_FocusArea_FocusAreaName", IsUnique = true)]
public partial class FocusArea
{
    [Key]
    public int FocusAreaID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string FocusAreaName { get; set; } = null!;

    public int FocusAreaStatusID { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? FocusAreaLocation { get; set; }

    public int DNRUplandRegionID { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? PlannedFootprintAcres { get; set; }

    [ForeignKey("DNRUplandRegionID")]
    [InverseProperty("FocusAreas")]
    public virtual DNRUplandRegion DNRUplandRegion { get; set; } = null!;

    [InverseProperty("FocusArea")]
    public virtual ICollection<FocusAreaLocationStaging> FocusAreaLocationStagings { get; set; } = new List<FocusAreaLocationStaging>();

    [InverseProperty("FocusArea")]
    public virtual ICollection<ProjectUpdate> ProjectUpdates { get; set; } = new List<ProjectUpdate>();

    [InverseProperty("FocusArea")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
