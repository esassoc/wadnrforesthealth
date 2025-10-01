using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectFundSourceAllocationRequest")]
[Index("ProjectID", "FundSourceAllocationID", Name = "AK_ProjectFundSourceAllocationRequest_ProjectID_FundSourceAllocationID", IsUnique = true)]
public partial class ProjectFundSourceAllocationRequest
{
    [Key]
    public int ProjectFundSourceAllocationRequestID { get; set; }

    public int ProjectID { get; set; }

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
    [InverseProperty("ProjectFundSourceAllocationRequests")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectFundSourceAllocationRequests")]
    public virtual Project Project { get; set; } = null!;
}
