using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectPriorityLandscape")]
[Index("ProjectID", "PriorityLandscapeID", Name = "AK_ProjectPriorityLandscape_ProjectID_PriorityLandscapeID", IsUnique = true)]
public partial class ProjectPriorityLandscape
{
    [Key]
    public int ProjectPriorityLandscapeID { get; set; }

    public int ProjectID { get; set; }

    public int PriorityLandscapeID { get; set; }

    [ForeignKey("PriorityLandscapeID")]
    [InverseProperty("ProjectPriorityLandscapes")]
    public virtual PriorityLandscape PriorityLandscape { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectPriorityLandscapes")]
    public virtual Project Project { get; set; } = null!;
}
