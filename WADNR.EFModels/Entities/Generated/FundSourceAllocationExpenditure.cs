using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceAllocationExpenditure")]
public partial class FundSourceAllocationExpenditure
{
    [Key]
    public int FundSourceAllocationExpenditureID { get; set; }

    public int FundSourceAllocationID { get; set; }

    public int? CostTypeID { get; set; }

    public int Biennium { get; set; }

    public int FiscalMonth { get; set; }

    public int CalendarYear { get; set; }

    public int CalendarMonth { get; set; }

    [Column(TypeName = "money")]
    public decimal ExpenditureAmount { get; set; }

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationExpenditures")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;
}
