using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectLocation")]
[Index("ProjectID", "ProjectLocationName", Name = "AK_ProjectLocation_ProjectID_ProjectLocationName", IsUnique = true)]
[Index("ProjectLocationGeometry", Name = "SPATIAL_ProjectLocation_ProjectLocationGeometry")]
public partial class ProjectLocation
{
    [Key]
    public int ProjectLocationID { get; set; }

    public int ProjectID { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry ProjectLocationGeometry { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? ProjectLocationNotes { get; set; }

    public int ProjectLocationTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ProjectLocationName { get; set; } = null!;

    public int? ArcGisObjectID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ArcGisGlobalID { get; set; }

    public int? ProgramID { get; set; }

    public bool? ImportedFromGisUpload { get; set; }

    public int? TemporaryTreatmentCacheID { get; set; }

    [ForeignKey("ProgramID")]
    [InverseProperty("ProjectLocations")]
    public virtual Program? Program { get; set; }

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectLocations")]
    public virtual Project Project { get; set; } = null!;
}
