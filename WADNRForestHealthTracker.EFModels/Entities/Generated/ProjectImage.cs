using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectImage")]
[Index("FileResourceID", "ProjectID", Name = "AK_ProjectImage_FileResourceID_ProjectID", IsUnique = true)]
public partial class ProjectImage
{
    [Key]
    public int ProjectImageID { get; set; }

    public int FileResourceID { get; set; }

    public int ProjectID { get; set; }

    public int? ProjectImageTimingID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Caption { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Credit { get; set; } = null!;

    public bool IsKeyPhoto { get; set; }

    public bool ExcludeFromFactSheet { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("ProjectImages")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectImages")]
    public virtual Project Project { get; set; } = null!;

    [InverseProperty("ProjectImage")]
    public virtual ICollection<ProjectImageUpdate> ProjectImageUpdates { get; set; } = new List<ProjectImageUpdate>();
}
