using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("Person")]
public partial class Person
{
    [Key]
    public int PersonID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? LastName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastActivityDate { get; set; }

    public bool IsActive { get; set; }

    public int? OrganizationID { get; set; }

    public bool ReceiveSupportEmails { get; set; }

    public Guid? ApiKey { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? MiddleName { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Notes { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? PersonAddress { get; set; }

    public int? AddedByPersonID { get; set; }

    public int? VendorID { get; set; }

    public bool? IsProgramManager { get; set; }

    public bool? CreatedAsPartOfBulkImport { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? GlobalID { get; set; }

    public int? ImpersonatedPersonID { get; set; }

    [ForeignKey("AddedByPersonID")]
    [InverseProperty("InverseAddedByPerson")]
    public virtual Person? AddedByPerson { get; set; }

    [InverseProperty("Person")]
    public virtual ICollection<AgreementPerson> AgreementPeople { get; set; } = new List<AgreementPerson>();

    [InverseProperty("Person")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("DNRUplandRegionCoordinator")]
    public virtual ICollection<DNRUplandRegion> DNRUplandRegions { get; set; } = new List<DNRUplandRegion>();

    [InverseProperty("CreatePerson")]
    public virtual ICollection<FileResource> FileResources { get; set; } = new List<FileResource>();

    [InverseProperty("Person")]
    public virtual ICollection<ForesterWorkUnit> ForesterWorkUnits { get; set; } = new List<ForesterWorkUnit>();

    [InverseProperty("ChangePerson")]
    public virtual ICollection<FundSourceAllocationChangeLog> FundSourceAllocationChangeLogs { get; set; } = new List<FundSourceAllocationChangeLog>();

    [InverseProperty("Person")]
    public virtual ICollection<FundSourceAllocationLikelyPerson> FundSourceAllocationLikelyPeople { get; set; } = new List<FundSourceAllocationLikelyPerson>();

    [InverseProperty("CreatedByPerson")]
    public virtual ICollection<FundSourceAllocationNote> FundSourceAllocationNoteCreatedByPeople { get; set; } = new List<FundSourceAllocationNote>();

    [InverseProperty("CreatedByPerson")]
    public virtual ICollection<FundSourceAllocationNoteInternal> FundSourceAllocationNoteInternalCreatedByPeople { get; set; } = new List<FundSourceAllocationNoteInternal>();

    [InverseProperty("LastUpdatedByPerson")]
    public virtual ICollection<FundSourceAllocationNoteInternal> FundSourceAllocationNoteInternalLastUpdatedByPeople { get; set; } = new List<FundSourceAllocationNoteInternal>();

    [InverseProperty("LastUpdatedByPerson")]
    public virtual ICollection<FundSourceAllocationNote> FundSourceAllocationNoteLastUpdatedByPeople { get; set; } = new List<FundSourceAllocationNote>();

    [InverseProperty("Person")]
    public virtual ICollection<FundSourceAllocationProgramManager> FundSourceAllocationProgramManagers { get; set; } = new List<FundSourceAllocationProgramManager>();

    [InverseProperty("FundSourceManager")]
    public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; } = new List<FundSourceAllocation>();

    [InverseProperty("CreatedByPerson")]
    public virtual ICollection<FundSourceNote> FundSourceNoteCreatedByPeople { get; set; } = new List<FundSourceNote>();

    [InverseProperty("CreatedByPerson")]
    public virtual ICollection<FundSourceNoteInternal> FundSourceNoteInternalCreatedByPeople { get; set; } = new List<FundSourceNoteInternal>();

    [InverseProperty("LastUpdatedByPerson")]
    public virtual ICollection<FundSourceNoteInternal> FundSourceNoteInternalLastUpdatedByPeople { get; set; } = new List<FundSourceNoteInternal>();

    [InverseProperty("LastUpdatedByPerson")]
    public virtual ICollection<FundSourceNote> FundSourceNoteLastUpdatedByPeople { get; set; } = new List<FundSourceNote>();

    [InverseProperty("GisUploadAttemptCreatePerson")]
    public virtual ICollection<GisUploadAttempt> GisUploadAttempts { get; set; } = new List<GisUploadAttempt>();

    [ForeignKey("ImpersonatedPersonID")]
    [InverseProperty("InverseImpersonatedPerson")]
    public virtual Person? ImpersonatedPerson { get; set; }

    [InverseProperty("Person")]
    public virtual ICollection<InteractionEventContact> InteractionEventContacts { get; set; } = new List<InteractionEventContact>();

    [InverseProperty("StaffPerson")]
    public virtual ICollection<InteractionEvent> InteractionEvents { get; set; } = new List<InteractionEvent>();

    [InverseProperty("AddedByPerson")]
    public virtual ICollection<Person> InverseAddedByPerson { get; set; } = new List<Person>();

    [InverseProperty("ImpersonatedPerson")]
    public virtual ICollection<Person> InverseImpersonatedPerson { get; set; } = new List<Person>();

    [InverseProperty("PreparedByPerson")]
    public virtual ICollection<InvoicePaymentRequest> InvoicePaymentRequests { get; set; } = new List<InvoicePaymentRequest>();

    [InverseProperty("Person")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("OrganizationID")]
    [InverseProperty("People")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("PrimaryContactPerson")]
    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    [InverseProperty("Person")]
    public virtual ICollection<PersonAllowedAuthenticator> PersonAllowedAuthenticators { get; set; } = new List<PersonAllowedAuthenticator>();

    [InverseProperty("Person")]
    public virtual ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();

    [InverseProperty("Person")]
    public virtual ICollection<PersonStewardOrganization> PersonStewardOrganizations { get; set; } = new List<PersonStewardOrganization>();

    [InverseProperty("Person")]
    public virtual ICollection<PersonStewardRegion> PersonStewardRegions { get; set; } = new List<PersonStewardRegion>();

    [InverseProperty("Person")]
    public virtual ICollection<PersonStewardTaxonomyBranch> PersonStewardTaxonomyBranches { get; set; } = new List<PersonStewardTaxonomyBranch>();

    [InverseProperty("SentToPerson")]
    public virtual ICollection<ProgramNotificationSent> ProgramNotificationSents { get; set; } = new List<ProgramNotificationSent>();

    [InverseProperty("Person")]
    public virtual ICollection<ProgramPerson> ProgramPeople { get; set; } = new List<ProgramPerson>();

    [InverseProperty("ProgramCreatePerson")]
    public virtual ICollection<Program> ProgramProgramCreatePeople { get; set; } = new List<Program>();

    [InverseProperty("ProgramLastUpdatedByPerson")]
    public virtual ICollection<Program> ProgramProgramLastUpdatedByPeople { get; set; } = new List<Program>();

    [InverseProperty("ProgramPrimaryContactPerson")]
    public virtual ICollection<Program> ProgramProgramPrimaryContactPeople { get; set; } = new List<Program>();

    [InverseProperty("CreatePerson")]
    public virtual ICollection<ProjectInternalNote> ProjectInternalNoteCreatePeople { get; set; } = new List<ProjectInternalNote>();

    [InverseProperty("UpdatePerson")]
    public virtual ICollection<ProjectInternalNote> ProjectInternalNoteUpdatePeople { get; set; } = new List<ProjectInternalNote>();

    [InverseProperty("Person")]
    public virtual ICollection<ProjectLocationStagingUpdate> ProjectLocationStagingUpdates { get; set; } = new List<ProjectLocationStagingUpdate>();

    [InverseProperty("Person")]
    public virtual ICollection<ProjectLocationStaging> ProjectLocationStagings { get; set; } = new List<ProjectLocationStaging>();

    [InverseProperty("CreatePerson")]
    public virtual ICollection<ProjectNote> ProjectNoteCreatePeople { get; set; } = new List<ProjectNote>();

    [InverseProperty("CreatePerson")]
    public virtual ICollection<ProjectNoteUpdate> ProjectNoteUpdateCreatePeople { get; set; } = new List<ProjectNoteUpdate>();

    [InverseProperty("UpdatePerson")]
    public virtual ICollection<ProjectNote> ProjectNoteUpdatePeople { get; set; } = new List<ProjectNote>();

    [InverseProperty("UpdatePerson")]
    public virtual ICollection<ProjectNoteUpdate> ProjectNoteUpdateUpdatePeople { get; set; } = new List<ProjectNoteUpdate>();

    [InverseProperty("Person")]
    public virtual ICollection<ProjectPerson> ProjectPeople { get; set; } = new List<ProjectPerson>();

    [InverseProperty("Person")]
    public virtual ICollection<ProjectPersonUpdate> ProjectPersonUpdates { get; set; } = new List<ProjectPersonUpdate>();

    [InverseProperty("ProposingPerson")]
    public virtual ICollection<Project> ProjectProposingPeople { get; set; } = new List<Project>();

    [InverseProperty("ReviewedByPerson")]
    public virtual ICollection<Project> ProjectReviewedByPeople { get; set; } = new List<Project>();

    [InverseProperty("LastUpdatePerson")]
    public virtual ICollection<ProjectUpdateBatch> ProjectUpdateBatches { get; set; } = new List<ProjectUpdateBatch>();

    [InverseProperty("UpdatePerson")]
    public virtual ICollection<ProjectUpdateHistory> ProjectUpdateHistories { get; set; } = new List<ProjectUpdateHistory>();

    [InverseProperty("RequestPerson")]
    public virtual ICollection<SupportRequestLog> SupportRequestLogs { get; set; } = new List<SupportRequestLog>();

    [InverseProperty("PrimaryContactPerson")]
    public virtual ICollection<SystemAttribute> SystemAttributes { get; set; } = new List<SystemAttribute>();

    [InverseProperty("LastProcessedPerson")]
    public virtual ICollection<TabularDataImport> TabularDataImportLastProcessedPeople { get; set; } = new List<TabularDataImport>();

    [InverseProperty("UploadPerson")]
    public virtual ICollection<TabularDataImport> TabularDataImportUploadPeople { get; set; } = new List<TabularDataImport>();

    [ForeignKey("VendorID")]
    [InverseProperty("People")]
    public virtual Vendor? Vendor { get; set; }
}
