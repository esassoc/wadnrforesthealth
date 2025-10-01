using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectClassification")]
[Index("ProjectID", "ClassificationID", Name = "AK_ProjectClassification_ProjectID_ClassificationID", IsUnique = true)]
public partial class ProjectClassification
{
    [Key]
    public int ProjectClassificationID { get; set; }

    public int ProjectID { get; set; }

    public int ClassificationID { get; set; }

    [StringLength(600)]
    [Unicode(false)]
    public string? ProjectClassificationNotes { get; set; }

    [ForeignKey("ClassificationID")]
    [InverseProperty("ProjectClassifications")]
    public virtual Classification Classification { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectClassifications")]
    public virtual Project Project { get; set; } = null!;
}
