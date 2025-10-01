using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("OrganizationType")]
public partial class OrganizationType
{
    [Key]
    public int OrganizationTypeID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string OrganizationTypeName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string OrganizationTypeAbbreviation { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string LegendColor { get; set; } = null!;

    public bool ShowOnProjectMaps { get; set; }

    public bool IsDefaultOrganizationType { get; set; }

    public bool IsFundingType { get; set; }

    [InverseProperty("OrganizationType")]
    public virtual ICollection<OrganizationTypeRelationshipType> OrganizationTypeRelationshipTypes { get; set; } = new List<OrganizationTypeRelationshipType>();

    [InverseProperty("OrganizationType")]
    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}
