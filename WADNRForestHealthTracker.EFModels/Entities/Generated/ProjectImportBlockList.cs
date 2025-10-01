using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectImportBlockList")]
public partial class ProjectImportBlockList
{
    [Key]
    public int ProjectImportBlockListID { get; set; }

    public int ProgramID { get; set; }

    [StringLength(140)]
    [Unicode(false)]
    public string? ProjectGisIdentifier { get; set; }

    [StringLength(140)]
    [Unicode(false)]
    public string? ProjectName { get; set; }

    public int? ProjectID { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Notes { get; set; }

    [ForeignKey("ProgramID")]
    [InverseProperty("ProjectImportBlockLists")]
    public virtual Program Program { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectImportBlockLists")]
    public virtual Project? Project { get; set; }
}
