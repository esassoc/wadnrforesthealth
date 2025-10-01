using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("GisUploadAttemptGisMetadataAttribute")]
public partial class GisUploadAttemptGisMetadataAttribute
{
    [Key]
    public int GisUploadAttemptGisMetadataAttributeID { get; set; }

    public int GisUploadAttemptID { get; set; }

    public int GisMetadataAttributeID { get; set; }

    public int SortOrder { get; set; }

    [ForeignKey("GisMetadataAttributeID")]
    [InverseProperty("GisUploadAttemptGisMetadataAttributes")]
    public virtual GisMetadataAttribute GisMetadataAttribute { get; set; } = null!;

    [ForeignKey("GisUploadAttemptID")]
    [InverseProperty("GisUploadAttemptGisMetadataAttributes")]
    public virtual GisUploadAttempt GisUploadAttempt { get; set; } = null!;
}
