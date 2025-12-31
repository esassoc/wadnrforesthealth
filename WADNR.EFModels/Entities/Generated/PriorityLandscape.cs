using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNR.EFModels.Entities;

[Table("PriorityLandscape")]
[Index("PriorityLandscapeName", Name = "AK_PriorityLandscape_PriorityLandscapeName", IsUnique = true)]
[Index("PriorityLandscapeLocation", Name = "SPATIAL_PriorityLandscape_PriorityLandscapeLocation")]
public partial class PriorityLandscape
{
    [Key]
    public int PriorityLandscapeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PriorityLandscapeName { get; set; } = null!;

    [Column(TypeName = "geometry")]
    public Geometry? PriorityLandscapeLocation { get; set; }

    [Unicode(false)]
    public string? PriorityLandscapeDescription { get; set; }

    public int? PlanYear { get; set; }

    [StringLength(2000)]
    [Unicode(false)]
    public string? PriorityLandscapeAboveMapText { get; set; }

    public int? PriorityLandscapeCategoryID { get; set; }

    [Unicode(false)]
    public string? PriorityLandscapeExternalResources { get; set; }

    [ForeignKey("PriorityLandscapeCategoryID")]
    [InverseProperty("PriorityLandscapes")]
    public virtual PriorityLandscapeCategory? PriorityLandscapeCategory { get; set; }

    [InverseProperty("PriorityLandscape")]
    public virtual ICollection<PriorityLandscapeFileResource> PriorityLandscapeFileResources { get; set; } = new List<PriorityLandscapeFileResource>();

    [InverseProperty("PriorityLandscape")]
    public virtual ICollection<ProjectPriorityLandscapeUpdate> ProjectPriorityLandscapeUpdates { get; set; } = new List<ProjectPriorityLandscapeUpdate>();

    [InverseProperty("PriorityLandscape")]
    public virtual ICollection<ProjectPriorityLandscape> ProjectPriorityLandscapes { get; set; } = new List<ProjectPriorityLandscape>();
}
