using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectType")]
public partial class ProjectType
{
    [Key]
    public int ProjectTypeID { get; set; }

    public int TaxonomyBranchID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ProjectTypeName { get; set; } = null!;

    [Unicode(false)]
    public string? ProjectTypeDescription { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? ProjectTypeCode { get; set; }

    [StringLength(7)]
    [Unicode(false)]
    public string? ThemeColor { get; set; }

    public int? ProjectTypeSortOrder { get; set; }

    public bool LimitVisibilityToAdmin { get; set; }

    [InverseProperty("ProjectType")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [ForeignKey("TaxonomyBranchID")]
    [InverseProperty("ProjectTypes")]
    public virtual TaxonomyBranch TaxonomyBranch { get; set; } = null!;
}
