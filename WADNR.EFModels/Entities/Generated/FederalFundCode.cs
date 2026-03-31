using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FederalFundCode")]
public partial class FederalFundCode
{
    [Key]
    public int FederalFundCodeID { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? FederalFundCodeAbbrev { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? FederalFundCodeProgram { get; set; }

    [InverseProperty("FederalFundCode")]
    public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; } = new List<FundSourceAllocation>();
}
