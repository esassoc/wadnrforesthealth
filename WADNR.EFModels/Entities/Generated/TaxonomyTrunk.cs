using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("TaxonomyTrunk")]
public partial class TaxonomyTrunk
{
    [Key]
    public int TaxonomyTrunkID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TaxonomyTrunkName { get; set; } = null!;

    [Unicode(false)]
    public string? TaxonomyTrunkDescription { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ThemeColor { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? TaxonomyTrunkCode { get; set; }

    public int? TaxonomyTrunkSortOrder { get; set; }

    [InverseProperty("TaxonomyTrunk")]
    public virtual ICollection<TaxonomyBranch> TaxonomyBranches { get; set; } = new List<TaxonomyBranch>();
}
