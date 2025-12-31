using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceFileResource")]
[Index("FundSourceID", "FileResourceID", Name = "AK_FundSourceFileResource_FundSourceID_FileResourceID", IsUnique = true)]
public partial class FundSourceFileResource
{
    [Key]
    public int FundSourceFileResourceID { get; set; }

    public int FundSourceID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? Description { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("FundSourceFileResources")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("FundSourceID")]
    [InverseProperty("FundSourceFileResources")]
    public virtual FundSource FundSource { get; set; } = null!;
}
