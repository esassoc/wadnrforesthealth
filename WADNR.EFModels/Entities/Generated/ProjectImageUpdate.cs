using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectImageUpdate")]
public partial class ProjectImageUpdate
{
    [Key]
    public int ProjectImageUpdateID { get; set; }

    public int? FileResourceID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int? ProjectImageTimingID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Caption { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Credit { get; set; } = null!;

    public bool IsKeyPhoto { get; set; }

    public bool ExcludeFromFactSheet { get; set; }

    public int? ProjectImageID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("ProjectImageUpdates")]
    public virtual FileResource? FileResource { get; set; }

    [ForeignKey("ProjectImageID")]
    [InverseProperty("ProjectImageUpdates")]
    public virtual ProjectImage? ProjectImage { get; set; }

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectImageUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
