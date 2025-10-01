using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("AgreementStatus")]
[Index("AgreementStatusName", Name = "AK_AgreementStatus_AgreementStatusName", IsUnique = true)]
public partial class AgreementStatus
{
    [Key]
    public int AgreementStatusID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string AgreementStatusName { get; set; } = null!;

    [InverseProperty("AgreementStatus")]
    public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
}
