using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("OrganizationBoundaryStaging")]
public partial class OrganizationBoundaryStaging
{
    [Key]
    public int OrganizationBoundaryStagingID { get; set; }

    public int OrganizationID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string FeatureClassName { get; set; } = null!;

    [Unicode(false)]
    public string GeoJson { get; set; } = null!;

    [ForeignKey("OrganizationID")]
    [InverseProperty("OrganizationBoundaryStagings")]
    public virtual Organization Organization { get; set; } = null!;
}
