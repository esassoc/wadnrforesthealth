using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("RelationshipType")]
public partial class RelationshipType
{
    [Key]
    public int RelationshipTypeID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string RelationshipTypeName { get; set; } = null!;

    public bool CanStewardProjects { get; set; }

    public bool IsPrimaryContact { get; set; }

    public bool CanOnlyBeRelatedOnceToAProject { get; set; }

    [StringLength(360)]
    [Unicode(false)]
    public string? RelationshipTypeDescription { get; set; }

    public bool ReportInAccomplishmentsDashboard { get; set; }

    public bool ShowOnFactSheet { get; set; }

    [InverseProperty("RelationshipTypeForDefaultOrganization")]
    public virtual ICollection<GisUploadSourceOrganization> GisUploadSourceOrganizations { get; set; } = new List<GisUploadSourceOrganization>();

    [InverseProperty("RelationshipType")]
    public virtual ICollection<OrganizationTypeRelationshipType> OrganizationTypeRelationshipTypes { get; set; } = new List<OrganizationTypeRelationshipType>();

    [InverseProperty("RelationshipType")]
    public virtual ICollection<ProjectOrganizationUpdate> ProjectOrganizationUpdates { get; set; } = new List<ProjectOrganizationUpdate>();

    [InverseProperty("RelationshipType")]
    public virtual ICollection<ProjectOrganization> ProjectOrganizations { get; set; } = new List<ProjectOrganization>();
}
