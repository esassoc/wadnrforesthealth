using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("CustomPage")]
public partial class CustomPage
{
    [Key]
    public int CustomPageID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CustomPageDisplayName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string CustomPageVanityUrl { get; set; } = null!;

    public int CustomPageDisplayTypeID { get; set; }

    [Unicode(false)]
    public string? CustomPageContent { get; set; }

    public int CustomPageNavigationSectionID { get; set; }

    [InverseProperty("CustomPage")]
    public virtual ICollection<CustomPageImage> CustomPageImages { get; set; } = new List<CustomPageImage>();
}
