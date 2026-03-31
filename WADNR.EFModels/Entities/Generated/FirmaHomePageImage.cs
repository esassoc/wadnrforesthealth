using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FirmaHomePageImage")]
public partial class FirmaHomePageImage
{
    [Key]
    public int FirmaHomePageImageID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string Caption { get; set; } = null!;

    public int SortOrder { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("FirmaHomePageImages")]
    public virtual FileResource FileResource { get; set; } = null!;
}
