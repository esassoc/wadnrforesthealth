using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectUpdateProgram")]
public partial class ProjectUpdateProgram
{
    [Key]
    public int ProjectUpdateProgramID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int ProgramID { get; set; }

    [ForeignKey("ProgramID")]
    [InverseProperty("ProjectUpdatePrograms")]
    public virtual Program Program { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectUpdatePrograms")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
