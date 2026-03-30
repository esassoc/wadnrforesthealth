using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("GisMetadataAttribute")]
public partial class GisMetadataAttribute
{
    [Key]
    public int GisMetadataAttributeID { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string GisMetadataAttributeName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? GisMetadataAttributeType { get; set; }

    [InverseProperty("GisMetadataAttribute")]
    public virtual ICollection<GisFeatureMetadataAttribute> GisFeatureMetadataAttributes { get; set; } = new List<GisFeatureMetadataAttribute>();

    [InverseProperty("GisMetadataAttribute")]
    public virtual ICollection<GisUploadAttemptGisMetadataAttribute> GisUploadAttemptGisMetadataAttributes { get; set; } = new List<GisUploadAttemptGisMetadataAttribute>();
}
