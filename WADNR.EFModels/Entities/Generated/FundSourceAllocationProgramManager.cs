using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceAllocationProgramManager")]
public partial class FundSourceAllocationProgramManager
{
    [Key]
    public int FundSourceAllocationProgramManagerID { get; set; }

    public int FundSourceAllocationID { get; set; }

    public int PersonID { get; set; }

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationProgramManagers")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("FundSourceAllocationProgramManagers")]
    public virtual Person Person { get; set; } = null!;
}
