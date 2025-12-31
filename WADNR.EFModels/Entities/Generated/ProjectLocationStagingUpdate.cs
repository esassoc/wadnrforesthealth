using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectLocationStagingUpdate")]
[Index("ProjectUpdateBatchID", "PersonID", "FeatureClassName", Name = "AK_ProjectLocationStagingUpdate_ProjectUpdateBatchID_PersonID_FeatureClassName", IsUnique = true)]
public partial class ProjectLocationStagingUpdate
{
    [Key]
    public int ProjectLocationStagingUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

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
    [InverseProperty("ProjectLocationStagingUpdates")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectLocationStagingUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
