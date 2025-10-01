using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("TaxonomyBranch")]
public partial class TaxonomyBranch
{
    [Key]
    public int TaxonomyBranchID { get; set; }

    public int TaxonomyTrunkID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TaxonomyBranchName { get; set; } = null!;

    [Unicode(false)]
    public string? TaxonomyBranchDescription { get; set; }

    [StringLength(7)]
    [Unicode(false)]
    public string? ThemeColor { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? TaxonomyBranchCode { get; set; }

    public int? TaxonomyBranchSortOrder { get; set; }

    [InverseProperty("TaxonomyBranch")]
    public virtual ICollection<PersonStewardTaxonomyBranch> PersonStewardTaxonomyBranches { get; set; } = new List<PersonStewardTaxonomyBranch>();

    [ForeignKey("TaxonomyTrunkID")]
    [InverseProperty("TaxonomyBranches")]
    public virtual TaxonomyTrunk TaxonomyTrunk { get; set; } = null!;
}
