using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectTag")]
[Index("ProjectID", "TagID", Name = "AK_ProjectTag_ProjectID_TagID", IsUnique = true)]
public partial class ProjectTag
{
    [Key]
    public int ProjectTagID { get; set; }

    public int ProjectID { get; set; }

    public int TagID { get; set; }

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectTags")]
    public virtual Project Project { get; set; } = null!;
}
