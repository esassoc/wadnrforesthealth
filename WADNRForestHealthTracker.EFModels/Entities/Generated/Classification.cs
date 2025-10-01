using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("Classification")]
public partial class Classification
{
    [Key]
    public int ClassificationID { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string ClassificationDescription { get; set; } = null!;

    [StringLength(7)]
    [Unicode(false)]
    public string ThemeColor { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? GoalStatement { get; set; }

    public int? KeyImageFileResourceID { get; set; }

    public int ClassificationSystemID { get; set; }

    public int? ClassificationSortOrder { get; set; }

    [ForeignKey("ClassificationSystemID")]
    [InverseProperty("Classifications")]
    public virtual ClassificationSystem ClassificationSystem { get; set; } = null!;

    [InverseProperty("Classification")]
    public virtual ICollection<ProjectClassification> ProjectClassifications { get; set; } = new List<ProjectClassification>();
}
