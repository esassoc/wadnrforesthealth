using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectDocumentUpdate")]
[Index("DisplayName", "ProjectUpdateBatchID", Name = "AK_ProjectDocumentUpdate_DisplayName_ProjectUpdateBatchID", IsUnique = true)]
[Index("ProjectUpdateBatchID", "FileResourceID", Name = "AK_ProjectDocumentUpdate_ProjectUpdateBatchID_FileResourceID", IsUnique = true)]
public partial class ProjectDocumentUpdate
{
    [Key]
    public int ProjectDocumentUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? Description { get; set; }

    public int? ProjectDocumentTypeID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("ProjectDocumentUpdates")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectDocumentUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
