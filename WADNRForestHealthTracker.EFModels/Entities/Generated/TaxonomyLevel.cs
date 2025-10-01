using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("TaxonomyLevel")]
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
}
