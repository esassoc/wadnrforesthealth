using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FileResource")]
[Index("FileResourceGUID", Name = "AK_FileResource_FileResourceGUID", IsUnique = true)]
public partial class FileResource
{
    [Key]
    public int FileResourceID { get; set; }

    public int FileResourceMimeTypeID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string OriginalBaseFilename { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string OriginalFileExtension { get; set; } = null!;

    public Guid FileResourceGUID { get; set; }

    public byte[] FileResourceData { get; set; } = null!;

    public int CreatePersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [ForeignKey("CreatePersonID")]
    [InverseProperty("FileResources")]
    public virtual Person CreatePerson { get; set; } = null!;

    [InverseProperty("FileResource")]
    public virtual ICollection<FirmaHomePageImage> FirmaHomePageImages { get; set; } = new List<FirmaHomePageImage>();

    [InverseProperty("FileResource")]
    public virtual ICollection<FirmaPageImage> FirmaPageImages { get; set; } = new List<FirmaPageImage>();

    [InverseProperty("FileResource")]
    public virtual ICollection<FundSourceAllocationFileResource> FundSourceAllocationFileResources { get; set; } = new List<FundSourceAllocationFileResource>();

    [InverseProperty("FileResource")]
    public virtual ICollection<FundSourceFileResource> FundSourceFileResources { get; set; } = new List<FundSourceFileResource>();

    [InverseProperty("FileResource")]
    public virtual InteractionEventFileResource? InteractionEventFileResource { get; set; }

    [InverseProperty("InvoiceFileResource")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("LogoFileResource")]
    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    [InverseProperty("FileResource")]
    public virtual PriorityLandscapeFileResource? PriorityLandscapeFileResource { get; set; }

    [InverseProperty("ProgramExampleGeospatialUploadFileResource")]
    public virtual ICollection<Program> ProgramProgramExampleGeospatialUploadFileResources { get; set; } = new List<Program>();

    [InverseProperty("ProgramFileResource")]
    public virtual ICollection<Program> ProgramProgramFileResources { get; set; } = new List<Program>();

    [InverseProperty("FileResource")]
    public virtual ICollection<ProjectDocumentUpdate> ProjectDocumentUpdates { get; set; } = new List<ProjectDocumentUpdate>();

    [InverseProperty("FileResource")]
    public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; } = new List<ProjectDocument>();

    [InverseProperty("FileResource")]
    public virtual ICollection<ProjectImageUpdate> ProjectImageUpdates { get; set; } = new List<ProjectImageUpdate>();

    [InverseProperty("FileResource")]
    public virtual ICollection<ProjectImage> ProjectImages { get; set; } = new List<ProjectImage>();

    [InverseProperty("BannerLogoFileResource")]
    public virtual ICollection<SystemAttribute> SystemAttributeBannerLogoFileResources { get; set; } = new List<SystemAttribute>();

    [InverseProperty("SquareLogoFileResource")]
    public virtual ICollection<SystemAttribute> SystemAttributeSquareLogoFileResources { get; set; } = new List<SystemAttribute>();

    public bool InBlobStorage { get; set; }
    public long? ContentLength { get; set; }
}
