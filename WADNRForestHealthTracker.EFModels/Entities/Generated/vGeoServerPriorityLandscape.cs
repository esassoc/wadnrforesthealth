using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Keyless]
public partial class vGeoServerPriorityLandscape
{
    public int PriorityLandscapeID { get; set; }

    public int PrimaryKey { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PriorityLandscapeName { get; set; } = null!;

    [Column(TypeName = "geometry")]
    public Geometry? PriorityLandscapeLocation { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? Ogr_Geometry { get; set; }

    public int? PriorityLandscapeCategoryID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PriorityLandscapeCategoryName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string MapColor { get; set; } = null!;
}
