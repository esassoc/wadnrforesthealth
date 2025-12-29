using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("WashingtonLegislativeDistrict")]
[Index("WashingtonLegislativeDistrictLocation", Name = "SPATIAL_WashingtonLegislativeDistrict_WashingtonLegislativeDistrictLocation")]
public partial class WashingtonLegislativeDistrict
{
    [Key]
    public int WashingtonLegislativeDistrictID { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry WashingtonLegislativeDistrictLocation { get; set; } = null!;

    public int WashingtonLegislativeDistrictNumber { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string WashingtonLegislativeDistrictName { get; set; } = null!;
}
