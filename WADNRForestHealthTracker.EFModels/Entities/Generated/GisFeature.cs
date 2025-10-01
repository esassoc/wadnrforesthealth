using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("GisFeature")]
public partial class GisFeature
{
    [Key]
    public int GisFeatureID { get; set; }

    public int GisUploadAttemptID { get; set; }

    public int GisImportFeatureKey { get; set; }

    public bool? IsValid { get; set; }

    [Column(TypeName = "decimal(38, 20)")]
    public decimal? CalculatedArea { get; set; }

    [InverseProperty("GisFeature")]
    public virtual ICollection<GisFeatureMetadataAttribute> GisFeatureMetadataAttributes { get; set; } = new List<GisFeatureMetadataAttribute>();

    [ForeignKey("GisUploadAttemptID")]
    [InverseProperty("GisFeatures")]
    public virtual GisUploadAttempt GisUploadAttempt { get; set; } = null!;
}
