using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("PersonStewardTaxonomyBranch")]
public partial class PersonStewardTaxonomyBranch
{
    [Key]
    public int PersonStewardTaxonomyBranchID { get; set; }

    public int PersonID { get; set; }

    public int TaxonomyBranchID { get; set; }

    [ForeignKey("PersonID")]
    [InverseProperty("PersonStewardTaxonomyBranches")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("TaxonomyBranchID")]
    [InverseProperty("PersonStewardTaxonomyBranches")]
    public virtual TaxonomyBranch TaxonomyBranch { get; set; } = null!;
}
