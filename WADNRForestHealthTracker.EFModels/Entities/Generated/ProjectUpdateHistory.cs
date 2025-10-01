using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectUpdateHistory")]
public partial class ProjectUpdateHistory
{
    [Key]
    public int ProjectUpdateHistoryID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int ProjectUpdateStateID { get; set; }

    public int UpdatePersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TransitionDate { get; set; }

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectUpdateHistories")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;

    [ForeignKey("UpdatePersonID")]
    [InverseProperty("ProjectUpdateHistories")]
    public virtual Person UpdatePerson { get; set; } = null!;
}
