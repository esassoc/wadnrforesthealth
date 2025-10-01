using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Keyless]
public partial class vGeoServerPriorityLandscape
{
    public int PriorityLandscapeID { get; set; }

    public int PrimaryKey { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PriorityLandscapeName { get; set; } = null!;

    public int? PriorityLandscapeCategoryID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PriorityLandscapeCategoryName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string MapColor { get; set; } = null!;
}
