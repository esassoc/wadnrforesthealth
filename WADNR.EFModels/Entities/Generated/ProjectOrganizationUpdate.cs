using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectOrganizationUpdate")]
public partial class ProjectOrganizationUpdate
{
    [Key]
    public int ProjectOrganizationUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int OrganizationID { get; set; }

    public int RelationshipTypeID { get; set; }

    [ForeignKey("OrganizationID")]
    [InverseProperty("ProjectOrganizationUpdates")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectOrganizationUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;

    [ForeignKey("RelationshipTypeID")]
    [InverseProperty("ProjectOrganizationUpdates")]
    public virtual RelationshipType RelationshipType { get; set; } = null!;
}
