using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FirmaPage")]
[Index("FirmaPageTypeID", Name = "AK_FirmaPage_FirmaPageTypeID", IsUnique = true)]
public partial class FirmaPage
{
    [Key]
    public int FirmaPageID { get; set; }

    public int FirmaPageTypeID { get; set; }

    [Unicode(false)]
    public string? FirmaPageContent { get; set; }

    [InverseProperty("FirmaPage")]
    public virtual ICollection<FirmaPageImage> FirmaPageImages { get; set; } = new List<FirmaPageImage>();
}
