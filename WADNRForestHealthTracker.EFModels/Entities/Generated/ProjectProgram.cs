using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectProgram")]
[Index("ProjectID", "ProgramID", Name = "AK_ProjectProgram_ProjectID_ProgramID", IsUnique = true)]
public partial class ProjectProgram
{
    [Key]
    public int ProjectProgramID { get; set; }

    public int ProjectID { get; set; }

    public int ProgramID { get; set; }

    [ForeignKey("ProgramID")]
    [InverseProperty("ProjectPrograms")]
    public virtual Program Program { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectPrograms")]
    public virtual Project Project { get; set; } = null!;
}
