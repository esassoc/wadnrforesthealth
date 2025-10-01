using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectOrganization")]
public partial class ProjectOrganization
{
    [Key]
    public int ProjectOrganizationID { get; set; }

    public int ProjectID { get; set; }

    public int OrganizationID { get; set; }

    public int RelationshipTypeID { get; set; }

    [ForeignKey("OrganizationID")]
    [InverseProperty("ProjectOrganizations")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectOrganizations")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("RelationshipTypeID")]
    [InverseProperty("ProjectOrganizations")]
    public virtual RelationshipType RelationshipType { get; set; } = null!;
}
