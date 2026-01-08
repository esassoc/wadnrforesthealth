using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNR.EFModels.Entities;

[Table("Project")]
[Index("FhtProjectNumber", Name = "AK_Project_FhtProjectNumber", IsUnique = true)]
public partial class Project
{
    [Key]
    public int ProjectID { get; set; }

    public int ProjectTypeID { get; set; }

    public int ProjectStageID { get; set; }

    [StringLength(140)]
    [Unicode(false)]
    public string ProjectName { get; set; } = null!;

    [StringLength(4000)]
    [Unicode(false)]
    public string? ProjectDescription { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CompletionDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? EstimatedTotalCost { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? ProjectLocationPoint { get; set; }

    public bool IsFeatured { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? ProjectLocationNotes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PlannedDate { get; set; }

    public int ProjectLocationSimpleTypeID { get; set; }

    public int ProjectApprovalStatusID { get; set; }

    public int? ProposingPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ProposingDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SubmissionDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ApprovalDate { get; set; }

    public int? ReviewedByPersonID { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? DefaultBoundingBox { get; set; }

    [Unicode(false)]
    public string? NoExpendituresToReportExplanation { get; set; }

    public int? FocusAreaID { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? NoRegionsExplanation { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpirationDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string FhtProjectNumber { get; set; } = null!;

    [StringLength(4000)]
    [Unicode(false)]
    public string? NoPriorityLandscapesExplanation { get; set; }

    public int? CreateGisUploadAttemptID { get; set; }

    public int? LastUpdateGisUploadAttemptID { get; set; }

    [StringLength(140)]
    [Unicode(false)]
    public string? ProjectGisIdentifier { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? ProjectFundingSourceNotes { get; set; }

    [StringLength(4000)]
    [Unicode(false)]
    public string? NoCountiesExplanation { get; set; }

    public int? PercentageMatch { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<AgreementProject> AgreementProjects { get; set; } = new List<AgreementProject>();

    [ForeignKey("CreateGisUploadAttemptID")]
    [InverseProperty("ProjectCreateGisUploadAttempts")]
    public virtual GisUploadAttempt? CreateGisUploadAttempt { get; set; }

    [ForeignKey("FocusAreaID")]
    [InverseProperty("Projects")]
    public virtual FocusArea? FocusArea { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<InteractionEventProject> InteractionEventProjects { get; set; } = new List<InteractionEventProject>();

    [InverseProperty("Project")]
    public virtual ICollection<InvoicePaymentRequest> InvoicePaymentRequests { get; set; } = new List<InvoicePaymentRequest>();

    [ForeignKey("LastUpdateGisUploadAttemptID")]
    [InverseProperty("ProjectLastUpdateGisUploadAttempts")]
    public virtual GisUploadAttempt? LastUpdateGisUploadAttempt { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<NotificationProject> NotificationProjects { get; set; } = new List<NotificationProject>();

    [InverseProperty("Project")]
    public virtual ICollection<ProgramNotificationSentProject> ProgramNotificationSentProjects { get; set; } = new List<ProgramNotificationSentProject>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectClassification> ProjectClassifications { get; set; } = new List<ProjectClassification>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectCounty> ProjectCounties { get; set; } = new List<ProjectCounty>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; } = new List<ProjectDocument>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectExternalLink> ProjectExternalLinks { get; set; } = new List<ProjectExternalLink>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectFundSourceAllocationRequest> ProjectFundSourceAllocationRequests { get; set; } = new List<ProjectFundSourceAllocationRequest>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectFundingSource> ProjectFundingSources { get; set; } = new List<ProjectFundingSource>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectImage> ProjectImages { get; set; } = new List<ProjectImage>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectImportBlockList> ProjectImportBlockLists { get; set; } = new List<ProjectImportBlockList>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectInternalNote> ProjectInternalNotes { get; set; } = new List<ProjectInternalNote>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectLocationStaging> ProjectLocationStagings { get; set; } = new List<ProjectLocationStaging>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectLocation> ProjectLocations { get; set; } = new List<ProjectLocation>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectNote> ProjectNotes { get; set; } = new List<ProjectNote>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectOrganization> ProjectOrganizations { get; set; } = new List<ProjectOrganization>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectPerson> ProjectPeople { get; set; } = new List<ProjectPerson>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectPriorityLandscape> ProjectPriorityLandscapes { get; set; } = new List<ProjectPriorityLandscape>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectProgram> ProjectPrograms { get; set; } = new List<ProjectProgram>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectRegion> ProjectRegions { get; set; } = new List<ProjectRegion>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();

    [ForeignKey("ProjectTypeID")]
    [InverseProperty("Projects")]
    public virtual ProjectType ProjectType { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<ProjectUpdateBatch> ProjectUpdateBatches { get; set; } = new List<ProjectUpdateBatch>();

    [ForeignKey("ProposingPersonID")]
    [InverseProperty("ProjectProposingPeople")]
    public virtual Person? ProposingPerson { get; set; }

    [ForeignKey("ReviewedByPersonID")]
    [InverseProperty("ProjectReviewedByPeople")]
    public virtual Person? ReviewedByPerson { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}
