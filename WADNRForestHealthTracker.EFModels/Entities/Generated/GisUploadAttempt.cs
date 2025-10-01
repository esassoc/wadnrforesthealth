using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("GisUploadAttempt")]
[Index("GisUploadAttemptCreatePersonID", Name = "IX_GisUploadAttempt_GisUploadAttemptCreatePersonID")]
public partial class GisUploadAttempt
{
    [Key]
    public int GisUploadAttemptID { get; set; }

    public int GisUploadSourceOrganizationID { get; set; }

    public int GisUploadAttemptCreatePersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime GisUploadAttemptCreateDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ImportTableName { get; set; }

    public bool? FileUploadSuccessful { get; set; }

    public bool? FeaturesSaved { get; set; }

    public bool? AttributesSaved { get; set; }

    public bool? AreaCalculationComplete { get; set; }

    public bool? ImportedToGeoJson { get; set; }

    [InverseProperty("GisUploadAttempt")]
    public virtual ICollection<GisFeature> GisFeatures { get; set; } = new List<GisFeature>();

    [ForeignKey("GisUploadAttemptCreatePersonID")]
    [InverseProperty("GisUploadAttempts")]
    public virtual Person GisUploadAttemptCreatePerson { get; set; } = null!;

    [InverseProperty("GisUploadAttempt")]
    public virtual ICollection<GisUploadAttemptGisMetadataAttribute> GisUploadAttemptGisMetadataAttributes { get; set; } = new List<GisUploadAttemptGisMetadataAttribute>();

    [ForeignKey("GisUploadSourceOrganizationID")]
    [InverseProperty("GisUploadAttempts")]
    public virtual GisUploadSourceOrganization GisUploadSourceOrganization { get; set; } = null!;

    [InverseProperty("CreateGisUploadAttempt")]
    public virtual ICollection<Project> ProjectCreateGisUploadAttempts { get; set; } = new List<Project>();

    [InverseProperty("LastUpdateGisUploadAttempt")]
    public virtual ICollection<Project> ProjectLastUpdateGisUploadAttempts { get; set; } = new List<Project>();
}
