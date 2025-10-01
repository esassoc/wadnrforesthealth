using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("TreatmentArea")]
public partial class TreatmentArea
{
    [Key]
    public int TreatmentAreaID { get; set; }

    public int? TemporaryTreatmentCacheID { get; set; }
}
