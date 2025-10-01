using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("GisFeatureMetadataAttribute")]
[Index("GisFeatureID", Name = "IDX_GisFeatureMetadataAttribute_GisFeatureID")]
[Index("GisMetadataAttributeID", Name = "IDX_GisFeatureMetadataAttribute_GisMetadataAttributeID")]
public partial class GisFeatureMetadataAttribute
{
    [Key]
    public int GisFeatureMetadataAttributeID { get; set; }

    public int GisFeatureID { get; set; }

    public int GisMetadataAttributeID { get; set; }

    [Unicode(false)]
    public string? GisFeatureMetadataAttributeValue { get; set; }

    [ForeignKey("GisFeatureID")]
    [InverseProperty("GisFeatureMetadataAttributes")]
    public virtual GisFeature GisFeature { get; set; } = null!;

    [ForeignKey("GisMetadataAttributeID")]
    [InverseProperty("GisFeatureMetadataAttributes")]
    public virtual GisMetadataAttribute GisMetadataAttribute { get; set; } = null!;
}
