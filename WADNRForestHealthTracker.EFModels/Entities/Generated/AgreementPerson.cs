using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("AgreementPerson")]
public partial class AgreementPerson
{
    [Key]
    public int AgreementPersonID { get; set; }

    public int AgreementID { get; set; }

    public int PersonID { get; set; }

    public int AgreementPersonRoleID { get; set; }

    [ForeignKey("AgreementID")]
    [InverseProperty("AgreementPeople")]
    public virtual Agreement Agreement { get; set; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("AgreementPeople")]
    public virtual Person Person { get; set; } = null!;
}
