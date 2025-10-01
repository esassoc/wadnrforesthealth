using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectCountyUpdate")]
[Index("ProjectUpdateBatchID", "CountyID", Name = "AK_ProjectCountyUpdate_ProjectUpdateBatchID_CountyID", IsUnique = true)]
public partial class ProjectCountyUpdate
{
    [Key]
    public int ProjectCountyUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int CountyID { get; set; }

    [ForeignKey("CountyID")]
    [InverseProperty("ProjectCountyUpdates")]
    public virtual County County { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectCountyUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
