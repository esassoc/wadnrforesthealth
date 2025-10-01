using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectFundSourceAllocationRequestUpdate")]
[Index("ProjectUpdateBatchID", "FundSourceAllocationID", Name = "AK_ProjectFundSourceAllocationRequestUpdate_ProjectUpdateBatchID_FundSourceAllocationID", IsUnique = true)]
public partial class ProjectFundSourceAllocationRequestUpdate
{
    [Key]
    public int ProjectFundSourceAllocationRequestUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int FundSourceAllocationID { get; set; }

    [Column(TypeName = "money")]
    public decimal? TotalAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? MatchAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? PayAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public bool ImportedFromTabularData { get; set; }

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("ProjectFundSourceAllocationRequestUpdates")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectFundSourceAllocationRequestUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
