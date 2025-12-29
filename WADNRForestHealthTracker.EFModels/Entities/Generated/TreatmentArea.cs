using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("TreatmentArea")]
[Index("TreatmentAreaFeature", Name = "SPATIAL_TreatmentArea_TreatmentAreaFeature")]
public partial class TreatmentArea
{
    [Key]
    public int TreatmentAreaID { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry TreatmentAreaFeature { get; set; } = null!;

    public int? TemporaryTreatmentCacheID { get; set; }
}
