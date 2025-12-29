using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Keyless]
public partial class vGeoServerCounty
{
    public int CountyID { get; set; }

    public int PrimaryKey { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CountyName { get; set; } = null!;

    [Column(TypeName = "geometry")]
    public Geometry? CountyFeature { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? Ogr_Geometry { get; set; }
}
