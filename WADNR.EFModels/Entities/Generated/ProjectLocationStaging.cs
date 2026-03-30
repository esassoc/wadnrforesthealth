using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectLocationStaging")]
[Index("ProjectID", "PersonID", "FeatureClassName", Name = "AK_ProjectLocationStaging_ProjectID_PersonID_FeatureClassName", IsUnique = true)]
public partial class ProjectLocationStaging
{
    [Key]
    public int ProjectLocationStagingID { get; set; }

    public int ProjectID { get; set; }

    public int PersonID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string FeatureClassName { get; set; } = null!;

    [Unicode(false)]
    public string GeoJson { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? SelectedProperty { get; set; }

    public bool ShouldImport { get; set; }

    [ForeignKey("PersonID")]
    [InverseProperty("ProjectLocationStagings")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectLocationStagings")]
    public virtual Project Project { get; set; } = null!;
}
