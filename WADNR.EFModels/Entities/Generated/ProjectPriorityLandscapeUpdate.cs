using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectPriorityLandscapeUpdate")]
[Index("ProjectUpdateBatchID", "PriorityLandscapeID", Name = "AK_ProjectPriorityLandscapeUpdate_ProjectUpdateBatchID_PriorityLandscapeID", IsUnique = true)]
public partial class ProjectPriorityLandscapeUpdate
{
    [Key]
    public int ProjectPriorityLandscapeUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int PriorityLandscapeID { get; set; }

    [ForeignKey("PriorityLandscapeID")]
    [InverseProperty("ProjectPriorityLandscapeUpdates")]
    public virtual PriorityLandscape PriorityLandscape { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectPriorityLandscapeUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
