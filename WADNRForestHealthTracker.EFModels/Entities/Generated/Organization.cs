using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("Organization")]
[Index("OrganizationName", Name = "AK_Organization_OrganizationName", IsUnique = true)]
[Index("OrganizationShortName", Name = "AK_Organization_OrganizationShortName", IsUnique = true)]
public partial class Organization
{
    [Key]
    public int OrganizationID { get; set; }

    public Guid? OrganizationGuid { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string OrganizationName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string OrganizationShortName { get; set; } = null!;

    public int? PrimaryContactPersonID { get; set; }

    public bool IsActive { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? OrganizationUrl { get; set; }

    public int? LogoFileResourceID { get; set; }

    public int OrganizationTypeID { get; set; }

    public int? VendorID { get; set; }

    public bool IsEditable { get; set; }

    [InverseProperty("Organization")]
    public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();

    [InverseProperty("Organization")]
    public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; } = new List<FundSourceAllocation>();

    [InverseProperty("Organization")]
    public virtual ICollection<FundSource> FundSources { get; set; } = new List<FundSource>();

    [InverseProperty("DefaultLeadImplementerOrganization")]
    public virtual ICollection<GisUploadSourceOrganization> GisUploadSourceOrganizations { get; set; } = new List<GisUploadSourceOrganization>();

    [ForeignKey("LogoFileResourceID")]
    [InverseProperty("Organizations")]
    public virtual FileResource? LogoFileResource { get; set; }

    [InverseProperty("Organization")]
    public virtual ICollection<OrganizationBoundaryStaging> OrganizationBoundaryStagings { get; set; } = new List<OrganizationBoundaryStaging>();

    [ForeignKey("OrganizationTypeID")]
    [InverseProperty("Organizations")]
    public virtual OrganizationType OrganizationType { get; set; } = null!;

    [InverseProperty("Organization")]
    public virtual ICollection<Person> People { get; set; } = new List<Person>();

    [InverseProperty("Organization")]
    public virtual ICollection<PersonStewardOrganization> PersonStewardOrganizations { get; set; } = new List<PersonStewardOrganization>();

    [ForeignKey("PrimaryContactPersonID")]
    [InverseProperty("Organizations")]
    public virtual Person? PrimaryContactPerson { get; set; }

    [InverseProperty("Organization")]
    public virtual ICollection<Program> Programs { get; set; } = new List<Program>();

    [InverseProperty("Organization")]
    public virtual ICollection<ProjectOrganizationUpdate> ProjectOrganizationUpdates { get; set; } = new List<ProjectOrganizationUpdate>();

    [InverseProperty("Organization")]
    public virtual ICollection<ProjectOrganization> ProjectOrganizations { get; set; } = new List<ProjectOrganization>();

    [ForeignKey("VendorID")]
    [InverseProperty("Organizations")]
    public virtual Vendor? Vendor { get; set; }
}
