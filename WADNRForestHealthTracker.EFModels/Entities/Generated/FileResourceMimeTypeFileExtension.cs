using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FileResourceMimeTypeFileExtension")]
[Index("FileResourceMimeTypeID", "FileResourceMimeTypeFileExtensionText", Name = "AK_FileResourceMimeTypeFileExtension_FileResourceMimeTypeID_FileResourceMimeTypeFileExtensionText", IsUnique = true)]
public partial class FileResourceMimeTypeFileExtension
{
    [Key]
    public int FileResourceMimeTypeFileExtensionID { get; set; }

    public int FileResourceMimeTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FileResourceMimeTypeFileExtensionText { get; set; } = null!;
}
