using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FocusAreaLocationStaging")]
public partial class FocusAreaLocationStaging
{
    [Key]
    public int FocusAreaLocationStagingID { get; set; }

    public int FocusAreaID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string FeatureClassName { get; set; } = null!;

    [Unicode(false)]
    public string GeoJson { get; set; } = null!;

    [ForeignKey("FocusAreaID")]
    [InverseProperty("FocusAreaLocationStagings")]
    public virtual FocusArea FocusArea { get; set; } = null!;
}
