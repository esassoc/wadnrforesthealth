using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("AgreementType")]
[Index("AgreementTypeName", Name = "AK_AgreementType_AgreementTypeName", IsUnique = true)]
public partial class AgreementType
{
    [Key]
    public int AgreementTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string AgreementTypeAbbrev { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string AgreementTypeName { get; set; } = null!;

    [InverseProperty("AgreementType")]
    public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
}
