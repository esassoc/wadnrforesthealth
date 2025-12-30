using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("Tag")]
public partial class Tag
{
    [Key]
    public int TagID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TagName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? TagDescription { get; set; }

    [InverseProperty("Tag")]
    public virtual ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
}
