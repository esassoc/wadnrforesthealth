using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FundSourceAllocationProgramIndexProjectCode")]
[Index("FundSourceAllocationID", "ProgramIndexID", "ProjectCodeID", Name = "AK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocationID_ProgramIndexID_ProjectCodeID", IsUnique = true)]
public partial class FundSourceAllocationProgramIndexProjectCode
{
    [Key]
    public int FundSourceAllocationProgramIndexProjectCodeID { get; set; }

    public int FundSourceAllocationID { get; set; }

    public int ProgramIndexID { get; set; }

    public int? ProjectCodeID { get; set; }

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationProgramIndexProjectCodes")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;

    [ForeignKey("ProgramIndexID")]
    [InverseProperty("FundSourceAllocationProgramIndexProjectCodes")]
    public virtual ProgramIndex ProgramIndex { get; set; } = null!;

    [ForeignKey("ProjectCodeID")]
    [InverseProperty("FundSourceAllocationProgramIndexProjectCodes")]
    public virtual ProjectCode? ProjectCode { get; set; }
}
