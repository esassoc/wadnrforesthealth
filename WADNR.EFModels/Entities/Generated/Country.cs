using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("Country")]
public partial class Country
{
    [Key]
    public int CountryID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string CountryName { get; set; } = null!;

    [StringLength(2)]
    [Unicode(false)]
    public string CountryAbbrev { get; set; } = null!;

    [InverseProperty("Country")]
    public virtual ICollection<StateProvince> StateProvinces { get; set; } = new List<StateProvince>();
}
