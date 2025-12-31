using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("TaxonomyLevel")]
[Index("TaxonomyLevelDisplayName", Name = "AK_TaxonomyLevel_TaxonomyLevelDisplayName", IsUnique = true)]
[Index("TaxonomyLevelName", Name = "AK_TaxonomyLevel_TaxonomyLevelName", IsUnique = true)]
public partial class TaxonomyLevel
{
    [Key]
    public int TaxonomyLevelID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TaxonomyLevelName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string TaxonomyLevelDisplayName { get; set; } = null!;

    [InverseProperty("AssociatePerfomanceMeasureTaxonomyLevel")]
    public virtual ICollection<SystemAttribute> SystemAttributeAssociatePerfomanceMeasureTaxonomyLevels { get; set; } = new List<SystemAttribute>();

    [InverseProperty("TaxonomyLevel")]
    public virtual ICollection<SystemAttribute> SystemAttributeTaxonomyLevels { get; set; } = new List<SystemAttribute>();
}
