using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceAllocationFileResource")]
[Index("FundSourceAllocationID", "FileResourceID", Name = "AK_FundSourceAllocationFileResource_FundSourceAllocationID_FileResourceID", IsUnique = true)]
public partial class FundSourceAllocationFileResource
{
    [Key]
    public int FundSourceAllocationFileResourceID { get; set; }

    public int FundSourceAllocationID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? Description { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("FundSourceAllocationFileResources")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationFileResources")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;
}
