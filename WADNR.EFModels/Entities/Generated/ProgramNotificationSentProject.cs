using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProgramNotificationSentProject")]
public partial class ProgramNotificationSentProject
{
    [Key]
    public int ProgramNotificationSentProjectID { get; set; }

    public int ProgramNotificationSentID { get; set; }

    public int ProjectID { get; set; }

    [ForeignKey("ProgramNotificationSentID")]
    [InverseProperty("ProgramNotificationSentProjects")]
    public virtual ProgramNotificationSent ProgramNotificationSent { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProgramNotificationSentProjects")]
    public virtual Project Project { get; set; } = null!;
}
