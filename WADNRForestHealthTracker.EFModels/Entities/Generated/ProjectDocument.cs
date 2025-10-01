using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectDocument")]
[Index("DisplayName", "ProjectID", Name = "AK_ProjectDocument_DisplayName_ProjectID", IsUnique = true)]
[Index("ProjectID", "FileResourceID", Name = "AK_ProjectDocument_ProjectID_FileResourceID", IsUnique = true)]
public partial class ProjectDocument
{
    [Key]
    public int ProjectDocumentID { get; set; }

    public int ProjectID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? Description { get; set; }

    public int? ProjectDocumentTypeID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("ProjectDocuments")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectDocuments")]
    public virtual Project Project { get; set; } = null!;
}
