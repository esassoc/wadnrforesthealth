using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceAllocationChangeLog")]
public partial class FundSourceAllocationChangeLog
{
    [Key]
    public int FundSourceAllocationChangeLogID { get; set; }

    public int FundSourceAllocationID { get; set; }

    [Column(TypeName = "money")]
    public decimal? FundSourceAllocationAmountOldValue { get; set; }

    [Column(TypeName = "money")]
    public decimal? FundSourceAllocationAmountNewValue { get; set; }

    [StringLength(2000)]
    [Unicode(false)]
    public string? FundSourceAllocationAmountNote { get; set; }

    public int ChangePersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ChangeDate { get; set; }

    [ForeignKey("ChangePersonID")]
    [InverseProperty("FundSourceAllocationChangeLogs")]
    public virtual Person ChangePerson { get; set; } = null!;

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationChangeLogs")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;
}
