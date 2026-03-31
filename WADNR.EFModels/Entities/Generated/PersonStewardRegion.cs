using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("PersonStewardRegion")]
public partial class PersonStewardRegion
{
    [Key]
    public int PersonStewardRegionID { get; set; }

    public int PersonID { get; set; }

    public int DNRUplandRegionID { get; set; }

    [ForeignKey("DNRUplandRegionID")]
    [InverseProperty("PersonStewardRegions")]
    public virtual DNRUplandRegion DNRUplandRegion { get; set; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("PersonStewardRegions")]
    public virtual Person Person { get; set; } = null!;
}
