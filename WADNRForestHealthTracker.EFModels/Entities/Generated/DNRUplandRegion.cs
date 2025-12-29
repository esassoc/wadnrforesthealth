using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("DNRUplandRegion")]
[Index("DNRUplandRegionName", Name = "AK_DNRUplandRegion_DNRUplandRegionName", IsUnique = true)]
public partial class DNRUplandRegion
{
    [Key]
    public int DNRUplandRegionID { get; set; }

    [StringLength(10)]
    public string? DNRUplandRegionAbbrev { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string DNRUplandRegionName { get; set; } = null!;

    [Column(TypeName = "geometry")]
    public Geometry? DNRUplandRegionLocation { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? RegionAddress { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? RegionCity { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? RegionState { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? RegionZip { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? RegionPhone { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? RegionEmail { get; set; }

    public int? DNRUplandRegionCoordinatorID { get; set; }

    [Unicode(false)]
    public string? RegionContent { get; set; }

    [InverseProperty("DNRUplandRegion")]
    public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();

    [InverseProperty("DNRUplandRegion")]
    public virtual ICollection<DNRUplandRegionContentImage> DNRUplandRegionContentImages { get; set; } = new List<DNRUplandRegionContentImage>();

    [ForeignKey("DNRUplandRegionCoordinatorID")]
    [InverseProperty("DNRUplandRegions")]
    public virtual Person? DNRUplandRegionCoordinator { get; set; }

    [InverseProperty("DNRUplandRegion")]
    public virtual ICollection<FocusArea> FocusAreas { get; set; } = new List<FocusArea>();

    [InverseProperty("DNRUplandRegion")]
    public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; } = new List<FundSourceAllocation>();

    [InverseProperty("DNRUplandRegion")]
    public virtual ICollection<PersonStewardRegion> PersonStewardRegions { get; set; } = new List<PersonStewardRegion>();

    [InverseProperty("DNRUplandRegion")]
    public virtual ICollection<ProjectRegionUpdate> ProjectRegionUpdates { get; set; } = new List<ProjectRegionUpdate>();

    [InverseProperty("DNRUplandRegion")]
    public virtual ICollection<ProjectRegion> ProjectRegions { get; set; } = new List<ProjectRegion>();
}
