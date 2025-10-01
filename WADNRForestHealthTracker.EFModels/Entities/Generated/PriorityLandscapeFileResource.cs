using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("PriorityLandscapeFileResource")]
[Index("FileResourceID", Name = "AK_PriorityLandscapeFileResource_FileResourceID", IsUnique = true)]
public partial class PriorityLandscapeFileResource
{
    [Key]
    public int PriorityLandscapeFileResourceID { get; set; }

    public int PriorityLandscapeID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? Description { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("PriorityLandscapeFileResource")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("PriorityLandscapeID")]
    [InverseProperty("PriorityLandscapeFileResources")]
    public virtual PriorityLandscape PriorityLandscape { get; set; } = null!;
}
