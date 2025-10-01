using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectRegionUpdate")]
[Index("ProjectUpdateBatchID", "DNRUplandRegionID", Name = "AK_ProjectRegionUpdate_ProjectUpdateBatchID_DNRUplandRegionID", IsUnique = true)]
public partial class ProjectRegionUpdate
{
    [Key]
    public int ProjectRegionUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int DNRUplandRegionID { get; set; }

    [ForeignKey("DNRUplandRegionID")]
    [InverseProperty("ProjectRegionUpdates")]
    public virtual DNRUplandRegion DNRUplandRegion { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectRegionUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
