using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("PersonRole")]
[Index("PersonID", "RoleID", Name = "AK_PersonRole_PersonID_RoleID", IsUnique = true)]
public partial class PersonRole
{
    [Key]
    public int PersonRoleID { get; set; }

    public int PersonID { get; set; }

    public int RoleID { get; set; }

    [ForeignKey("PersonID")]
    [InverseProperty("PersonRoles")]
    public virtual Person Person { get; set; } = null!;
}
