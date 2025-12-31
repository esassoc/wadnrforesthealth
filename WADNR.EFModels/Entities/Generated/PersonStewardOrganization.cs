using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("PersonStewardOrganization")]
public partial class PersonStewardOrganization
{
    [Key]
    public int PersonStewardOrganizationID { get; set; }

    public int PersonID { get; set; }

    public int OrganizationID { get; set; }

    [ForeignKey("OrganizationID")]
    [InverseProperty("PersonStewardOrganizations")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("PersonStewardOrganizations")]
    public virtual Person Person { get; set; } = null!;
}
