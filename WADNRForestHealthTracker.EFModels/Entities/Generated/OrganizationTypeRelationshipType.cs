using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("OrganizationTypeRelationshipType")]
public partial class OrganizationTypeRelationshipType
{
    [Key]
    public int OrganizationTypeRelationshipTypeID { get; set; }

    public int OrganizationTypeID { get; set; }

    public int RelationshipTypeID { get; set; }

    [ForeignKey("OrganizationTypeID")]
    [InverseProperty("OrganizationTypeRelationshipTypes")]
    public virtual OrganizationType OrganizationType { get; set; } = null!;

    [ForeignKey("RelationshipTypeID")]
    [InverseProperty("OrganizationTypeRelationshipTypes")]
    public virtual RelationshipType RelationshipType { get; set; } = null!;
}
