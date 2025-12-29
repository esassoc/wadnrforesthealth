using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ForesterWorkUnit")]
[Index("ForesterWorkUnitLocation", Name = "SPATIAL_ForesterWorkUnit_ForesterWorkUnitLocation")]
public partial class ForesterWorkUnit
{
    [Key]
    public int ForesterWorkUnitID { get; set; }

    public int ForesterRoleID { get; set; }

    public int? PersonID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ForesterWorkUnitName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? RegionName { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry ForesterWorkUnitLocation { get; set; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("ForesterWorkUnits")]
    public virtual Person? Person { get; set; }
}
