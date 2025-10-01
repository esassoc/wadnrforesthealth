using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectRegion")]
[Index("ProjectID", "DNRUplandRegionID", Name = "AK_ProjectRegion_ProjectID_DNRUplandRegionID", IsUnique = true)]
public partial class ProjectRegion
{
    [Key]
    public int ProjectRegionID { get; set; }

    public int ProjectID { get; set; }

    public int DNRUplandRegionID { get; set; }

    [ForeignKey("DNRUplandRegionID")]
    [InverseProperty("ProjectRegions")]
    public virtual DNRUplandRegion DNRUplandRegion { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectRegions")]
    public virtual Project Project { get; set; } = null!;
}
