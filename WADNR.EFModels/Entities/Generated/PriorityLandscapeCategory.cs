using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("PriorityLandscapeCategory")]
public partial class PriorityLandscapeCategory
{
    [Key]
    public int PriorityLandscapeCategoryID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PriorityLandscapeCategoryName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string PriorityLandscapeCategoryDisplayName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string PriorityLandscapeCategoryMapLayerColor { get; set; } = null!;

    [InverseProperty("PriorityLandscapeCategory")]
    public virtual ICollection<PriorityLandscape> PriorityLandscapes { get; set; } = new List<PriorityLandscape>();
}
