using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ExternalMapLayer")]
public partial class ExternalMapLayer
{
    [Key]
    public int ExternalMapLayerID { get; set; }

    [StringLength(75)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string LayerUrl { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? LayerDescription { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? FeatureNameField { get; set; }

    public bool DisplayOnPriorityLandscape { get; set; }

    public bool DisplayOnProjectMap { get; set; }

    public bool DisplayOnAllOthers { get; set; }

    public bool IsActive { get; set; }

    public bool IsTiledMapService { get; set; }
}
