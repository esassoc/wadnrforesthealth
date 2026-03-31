using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceAllocationBudgetLineItem")]
[Index("FundSourceAllocationID", "CostTypeID", Name = "AK_FundSourceAllocationBudgetLineItem_FundSourceAllocationID_CostTypeID", IsUnique = true)]
public partial class FundSourceAllocationBudgetLineItem
{
    [Key]
    public int FundSourceAllocationBudgetLineItemID { get; set; }

    public int FundSourceAllocationID { get; set; }

    public int CostTypeID { get; set; }

    [Column(TypeName = "money")]
    public decimal FundSourceAllocationBudgetLineItemAmount { get; set; }

    [Unicode(false)]
    public string? FundSourceAllocationBudgetLineItemNote { get; set; }

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationBudgetLineItems")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;
}
