using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceImage")]
[Index("FileResourceID", "FundSourceID", Name = "AK_FundSourceImage_FileResourceID_FundSourceID", IsUnique = true)]
public partial class FundSourceImage
{
    [Key]
    public int FundSourceImageID { get; set; }

    public int FileResourceID { get; set; }

    public int FundSourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Caption { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Credit { get; set; } = null!;

    public bool IsKeyPhoto { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("FundSourceImages")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("FundSourceID")]
    [InverseProperty("FundSourceImages")]
    public virtual FundSource FundSource { get; set; } = null!;
}
