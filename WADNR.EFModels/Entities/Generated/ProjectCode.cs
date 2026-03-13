using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectCode")]
[Index("ProjectCodeName", Name = "AK_ProjectCode_ProjectCodeName", IsUnique = true)]
public partial class ProjectCode
{
    [Key]
    public int ProjectCodeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ProjectCodeName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? ProjectCodeTitle { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    public DateOnly? ProjectStartDate { get; set; }

    public DateOnly? ProjectEndDate { get; set; }

    [InverseProperty("ProjectCode")]
    public virtual ICollection<FundSourceAllocationProgramIndexProjectCode> FundSourceAllocationProgramIndexProjectCodes { get; set; } = new List<FundSourceAllocationProgramIndexProjectCode>();

    [InverseProperty("ProjectCode")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
