using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("County")]
[Index("CountyName", "StateProvinceID", Name = "AK_County_CountyName_StateProvinceID", IsUnique = true)]
public partial class County
{
    [StringLength(100)]
    [Unicode(false)]
    public string CountyName { get; set; } = null!;

    public int StateProvinceID { get; set; }

    [Key]
    public int CountyID { get; set; }

    [InverseProperty("County")]
    public virtual ICollection<ProjectCounty> ProjectCounties { get; set; } = new List<ProjectCounty>();

    [InverseProperty("County")]
    public virtual ICollection<ProjectCountyUpdate> ProjectCountyUpdates { get; set; } = new List<ProjectCountyUpdate>();

    [ForeignKey("StateProvinceID")]
    [InverseProperty("Counties")]
    public virtual StateProvince StateProvince { get; set; } = null!;
}
