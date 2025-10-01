using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Keyless]
public partial class vTotalTreatedAcresByProject
{
    public int ProjectID { get; set; }

    [Column(TypeName = "decimal(38, 10)")]
    public decimal? TotalTreatedAcres { get; set; }
}
