using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ForesterWorkUnit")]
public partial class ForesterWorkUnit
{
    [Key]
    public int ForesterWorkUnitID { get; set; }

    public int ForesterRoleID { get; set; }

    public int? PersonID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ForesterWorkUnitName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? RegionName { get; set; }

    [ForeignKey("PersonID")]
    [InverseProperty("ForesterWorkUnits")]
    public virtual Person? Person { get; set; }
}
