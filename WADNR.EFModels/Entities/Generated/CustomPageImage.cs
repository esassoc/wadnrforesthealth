using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("CustomPageImage")]
public partial class CustomPageImage
{
    [Key]
    public int CustomPageImageID { get; set; }

    public int CustomPageID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("CustomPageID")]
    [InverseProperty("CustomPageImages")]
    public virtual CustomPage CustomPage { get; set; } = null!;

    [ForeignKey("FileResourceID")]
    [InverseProperty("CustomPageImages")]
    public virtual FileResource FileResource { get; set; } = null!;
}
