using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectUpdateProgram")]
public partial class ProjectUpdateProgram
{
    [Key]
    public int ProjectUpdateProgramID { get; set; }

    public int ProgramID { get; set; }

    public int ProjectUpdateBatchID { get; set; }
}
