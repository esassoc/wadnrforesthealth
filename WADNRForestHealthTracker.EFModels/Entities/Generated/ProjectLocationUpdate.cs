using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectLocationUpdate")]
[Index("ProjectUpdateBatchID", "ProjectLocationUpdateName", Name = "AK_ProjectLocationUpdate_ProjectUpdateBatchID_ProjectLocationUpdateName", IsUnique = true)]
public partial class ProjectLocationUpdate
{
    [Key]
    public int ProjectLocationUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ProjectLocationUpdateNotes { get; set; }

    public int ProjectLocationTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ProjectLocationUpdateName { get; set; } = null!;

    public int? ArcGisObjectID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ArcGisGlobalID { get; set; }

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectLocationUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;

    [InverseProperty("ProjectLocationUpdate")]
    public virtual ICollection<TreatmentUpdate> TreatmentUpdates { get; set; } = new List<TreatmentUpdate>();
}
