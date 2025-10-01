using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectCounty")]
[Index("ProjectID", "CountyID", Name = "AK_ProjectCounty_ProjectID_CountyID", IsUnique = true)]
public partial class ProjectCounty
{
    [Key]
    public int ProjectCountyID { get; set; }

    public int ProjectID { get; set; }

    public int CountyID { get; set; }

    [ForeignKey("CountyID")]
    [InverseProperty("ProjectCounties")]
    public virtual County County { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectCounties")]
    public virtual Project Project { get; set; } = null!;
}
