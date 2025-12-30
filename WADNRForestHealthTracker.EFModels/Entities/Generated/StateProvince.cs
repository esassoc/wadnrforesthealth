using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("StateProvince")]
[Index("StateProvinceAbbreviation", Name = "AK_StateProvince_StateProvinceAbbreviation", IsUnique = true)]
[Index("StateProvinceName", Name = "AK_StateProvince_StateProvinceName", IsUnique = true)]
[Index("StateProvinceFeature", Name = "SPATIAL_StateProvince_StateProvinceFeature")]
public partial class StateProvince
{
    [Key]
    public int StateProvinceID { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string StateProvinceAbbreviation { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string StateProvinceName { get; set; } = null!;

    public bool IsBpaRelevant { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? SouthWestLatitude { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? SouthWestLongitude { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? NorthEastLatitude { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? NorthEastLongitude { get; set; }

    public int? MapObjectID { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? StateProvinceFeature { get; set; }

    public int CountryID { get; set; }

    [InverseProperty("StateProvince")]
    public virtual ICollection<County> Counties { get; set; } = new List<County>();

    [ForeignKey("CountryID")]
    [InverseProperty("StateProvinces")]
    public virtual Country Country { get; set; } = null!;
}
