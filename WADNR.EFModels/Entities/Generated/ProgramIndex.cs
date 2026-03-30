using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProgramIndex")]
[Index("ProgramIndexCode", "Biennium", Name = "AK_ProgramIndex_ProgramIndexCode_Biennium", IsUnique = true)]
[Index("ProgramIndexTitle", "Biennium", Name = "AK_ProgramIndex_ProgramIndexTitle_Biennium", IsUnique = true)]
public partial class ProgramIndex
{
    [Key]
    public int ProgramIndexID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ProgramIndexCode { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string ProgramIndexTitle { get; set; } = null!;

    public int Biennium { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Activity { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Program { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Subprogram { get; set; }

    [Unicode(false)]
    public string? Subactivity { get; set; }

    [InverseProperty("ProgramIndex")]
    public virtual ICollection<FundSourceAllocationProgramIndexProjectCode> FundSourceAllocationProgramIndexProjectCodes { get; set; } = new List<FundSourceAllocationProgramIndexProjectCode>();

    [InverseProperty("ProgramIndex")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
