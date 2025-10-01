using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectFundingSourceUpdate")]
public partial class ProjectFundingSourceUpdate
{
    [Key]
    public int ProjectFundingSourceUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int FundingSourceID { get; set; }

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectFundingSourceUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
