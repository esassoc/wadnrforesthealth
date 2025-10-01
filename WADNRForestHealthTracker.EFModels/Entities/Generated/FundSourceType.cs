using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FundSourceType")]
[Index("FundSourceTypeName", Name = "AK_FundSourceType_FundSourceTypeName", IsUnique = true)]
public partial class FundSourceType
{
    [Key]
    public int FundSourceTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FundSourceTypeName { get; set; } = null!;

    [InverseProperty("FundSourceType")]
    public virtual ICollection<FundSource> FundSources { get; set; } = new List<FundSource>();
}
