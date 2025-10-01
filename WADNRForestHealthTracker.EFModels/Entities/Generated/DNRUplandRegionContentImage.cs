using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("DNRUplandRegionContentImage")]
public partial class DNRUplandRegionContentImage
{
    [Key]
    public int DNRUplandRegionContentImageID { get; set; }

    public int DNRUplandRegionID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("DNRUplandRegionID")]
    [InverseProperty("DNRUplandRegionContentImages")]
    public virtual DNRUplandRegion DNRUplandRegion { get; set; } = null!;
}
