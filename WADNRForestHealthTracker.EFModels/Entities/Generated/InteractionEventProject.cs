using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("InteractionEventProject")]
[Index("InteractionEventID", "ProjectID", Name = "AK_InteractionEventProject_InteractionEventID_ProjectID", IsUnique = true)]
public partial class InteractionEventProject
{
    [Key]
    public int InteractionEventProjectID { get; set; }

    public int InteractionEventID { get; set; }

    public int ProjectID { get; set; }

    [ForeignKey("InteractionEventID")]
    [InverseProperty("InteractionEventProjects")]
    public virtual InteractionEvent InteractionEvent { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("InteractionEventProjects")]
    public virtual Project Project { get; set; } = null!;
}
