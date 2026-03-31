using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FirmaPageImage")]
[Index("FirmaPageImageID", "FileResourceID", Name = "AK_FirmaPageImage_FirmaPageImageID_FileResourceID", IsUnique = true)]
public partial class FirmaPageImage
{
    [Key]
    public int FirmaPageImageID { get; set; }

    public int FirmaPageID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("FirmaPageImages")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("FirmaPageID")]
    [InverseProperty("FirmaPageImages")]
    public virtual FirmaPage FirmaPage { get; set; } = null!;
}
