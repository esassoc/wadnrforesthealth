using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("SystemAttribute")]
public partial class SystemAttribute
{
    [Key]
    public int SystemAttributeID { get; set; }

    public int MinimumYear { get; set; }

    public int? PrimaryContactPersonID { get; set; }

    public int? SquareLogoFileResourceID { get; set; }

    public int? BannerLogoFileResourceID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? RecaptchaPublicKey { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? RecaptchaPrivateKey { get; set; }

    public bool ShowApplicationsToThePublic { get; set; }

    public int TaxonomyLevelID { get; set; }

    public int AssociatePerfomanceMeasureTaxonomyLevelID { get; set; }

    public bool IsActive { get; set; }

    public bool ShowLeadImplementerLogoOnFactSheet { get; set; }

    public bool EnableAccomplishmentsDashboard { get; set; }

    public int? ProjectStewardshipAreaTypeID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string SocrataAppToken { get; set; } = null!;

    [ForeignKey("BannerLogoFileResourceID")]
    [InverseProperty("SystemAttributeBannerLogoFileResources")]
    public virtual FileResource? BannerLogoFileResource { get; set; }

    [ForeignKey("PrimaryContactPersonID")]
    [InverseProperty("SystemAttributes")]
    public virtual Person? PrimaryContactPerson { get; set; }

    [ForeignKey("SquareLogoFileResourceID")]
    [InverseProperty("SystemAttributeSquareLogoFileResources")]
    public virtual FileResource? SquareLogoFileResource { get; set; }
}
