using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FundSourceAllocationPriority")]
public partial class FundSourceAllocationPriority
{
    [Key]
    public int FundSourceAllocationPriorityID { get; set; }

    public int FundSourceAllocationPriorityNumber { get; set; }

    [StringLength(8)]
    [Unicode(false)]
    public string FundSourceAllocationPriorityColor { get; set; } = null!;

    [InverseProperty("FundSourceAllocationPriority")]
    public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; } = new List<FundSourceAllocation>();
}
