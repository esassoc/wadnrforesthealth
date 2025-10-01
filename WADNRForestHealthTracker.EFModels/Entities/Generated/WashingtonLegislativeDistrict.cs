using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("WashingtonLegislativeDistrict")]
public partial class WashingtonLegislativeDistrict
{
    [Key]
    public int WashingtonLegislativeDistrictID { get; set; }

    public int WashingtonLegislativeDistrictNumber { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string WashingtonLegislativeDistrictName { get; set; } = null!;
}
