using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

public partial class WADNRDbContext : DbContext
{
    public WADNRDbContext(DbContextOptions<WADNRDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agreement> Agreements { get; set; }

    public virtual DbSet<AgreementFundSourceAllocation> AgreementFundSourceAllocations { get; set; }

    public virtual DbSet<AgreementPerson> AgreementPeople { get; set; }

    public virtual DbSet<AgreementProject> AgreementProjects { get; set; }

    public virtual DbSet<AgreementStatus> AgreementStatuses { get; set; }

    public virtual DbSet<AgreementType> AgreementTypes { get; set; }

    public virtual DbSet<ArcOnlineFinanceApiRawJsonImport> ArcOnlineFinanceApiRawJsonImports { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Authenticator> Authenticators { get; set; }

    public virtual DbSet<Classification> Classifications { get; set; }

    public virtual DbSet<ClassificationSystem> ClassificationSystems { get; set; }

    public virtual DbSet<CostTypeDatamartMapping> CostTypeDatamartMappings { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<County> Counties { get; set; }

    public virtual DbSet<CustomPage> CustomPages { get; set; }

    public virtual DbSet<CustomPageImage> CustomPageImages { get; set; }

    public virtual DbSet<DNRUplandRegion> DNRUplandRegions { get; set; }

    public virtual DbSet<DNRUplandRegionContentImage> DNRUplandRegionContentImages { get; set; }

    public virtual DbSet<ExternalMapLayer> ExternalMapLayers { get; set; }

    public virtual DbSet<FederalFundCode> FederalFundCodes { get; set; }

    public virtual DbSet<FieldDefinitionDatum> FieldDefinitionData { get; set; }

    public virtual DbSet<FieldDefinitionDatumImage> FieldDefinitionDatumImages { get; set; }

    public virtual DbSet<FileResource> FileResources { get; set; }

    public virtual DbSet<FileResourceMimeTypeFileExtension> FileResourceMimeTypeFileExtensions { get; set; }

    public virtual DbSet<FindYourForesterQuestion> FindYourForesterQuestions { get; set; }

    public virtual DbSet<FirmaHomePageImage> FirmaHomePageImages { get; set; }

    public virtual DbSet<FirmaPage> FirmaPages { get; set; }

    public virtual DbSet<FirmaPageImage> FirmaPageImages { get; set; }

    public virtual DbSet<FocusArea> FocusAreas { get; set; }

    public virtual DbSet<FocusAreaLocationStaging> FocusAreaLocationStagings { get; set; }

    public virtual DbSet<ForesterWorkUnit> ForesterWorkUnits { get; set; }

    public virtual DbSet<FundSource> FundSources { get; set; }

    public virtual DbSet<FundSourceAllocation> FundSourceAllocations { get; set; }

    public virtual DbSet<FundSourceAllocationBudgetLineItem> FundSourceAllocationBudgetLineItems { get; set; }

    public virtual DbSet<FundSourceAllocationChangeLog> FundSourceAllocationChangeLogs { get; set; }

    public virtual DbSet<FundSourceAllocationExpenditure> FundSourceAllocationExpenditures { get; set; }

    public virtual DbSet<FundSourceAllocationExpenditureJsonStage> FundSourceAllocationExpenditureJsonStages { get; set; }

    public virtual DbSet<FundSourceAllocationFileResource> FundSourceAllocationFileResources { get; set; }

    public virtual DbSet<FundSourceAllocationLikelyPerson> FundSourceAllocationLikelyPeople { get; set; }

    public virtual DbSet<FundSourceAllocationNote> FundSourceAllocationNotes { get; set; }

    public virtual DbSet<FundSourceAllocationNoteInternal> FundSourceAllocationNoteInternals { get; set; }

    public virtual DbSet<FundSourceAllocationPriority> FundSourceAllocationPriorities { get; set; }

    public virtual DbSet<FundSourceAllocationProgramIndexProjectCode> FundSourceAllocationProgramIndexProjectCodes { get; set; }

    public virtual DbSet<FundSourceAllocationProgramManager> FundSourceAllocationProgramManagers { get; set; }

    public virtual DbSet<FundSourceFileResource> FundSourceFileResources { get; set; }

    public virtual DbSet<FundSourceNote> FundSourceNotes { get; set; }

    public virtual DbSet<FundSourceNoteInternal> FundSourceNoteInternals { get; set; }

    public virtual DbSet<FundSourceType> FundSourceTypes { get; set; }

    public virtual DbSet<GisCrossWalkDefault> GisCrossWalkDefaults { get; set; }

    public virtual DbSet<GisDefaultMapping> GisDefaultMappings { get; set; }

    public virtual DbSet<GisExcludeIncludeColumn> GisExcludeIncludeColumns { get; set; }

    public virtual DbSet<GisExcludeIncludeColumnValue> GisExcludeIncludeColumnValues { get; set; }

    public virtual DbSet<GisFeature> GisFeatures { get; set; }

    public virtual DbSet<GisFeatureMetadataAttribute> GisFeatureMetadataAttributes { get; set; }

    public virtual DbSet<GisMetadataAttribute> GisMetadataAttributes { get; set; }

    public virtual DbSet<GisUploadAttempt> GisUploadAttempts { get; set; }

    public virtual DbSet<GisUploadAttemptGisMetadataAttribute> GisUploadAttemptGisMetadataAttributes { get; set; }

    public virtual DbSet<GisUploadProgramMergeGrouping> GisUploadProgramMergeGroupings { get; set; }

    public virtual DbSet<GisUploadSourceOrganization> GisUploadSourceOrganizations { get; set; }

    public virtual DbSet<InteractionEvent> InteractionEvents { get; set; }

    public virtual DbSet<InteractionEventContact> InteractionEventContacts { get; set; }

    public virtual DbSet<InteractionEventFileResource> InteractionEventFileResources { get; set; }

    public virtual DbSet<InteractionEventProject> InteractionEventProjects { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceApprovalStatus> InvoiceApprovalStatuses { get; set; }

    public virtual DbSet<InvoicePaymentRequest> InvoicePaymentRequests { get; set; }

    public virtual DbSet<LoaStage> LoaStages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationProject> NotificationProjects { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<OrganizationBoundaryStaging> OrganizationBoundaryStagings { get; set; }

    public virtual DbSet<OrganizationType> OrganizationTypes { get; set; }

    public virtual DbSet<OrganizationTypeRelationshipType> OrganizationTypeRelationshipTypes { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<PersonAllowedAuthenticator> PersonAllowedAuthenticators { get; set; }

    public virtual DbSet<PersonRole> PersonRoles { get; set; }

    public virtual DbSet<PersonStewardOrganization> PersonStewardOrganizations { get; set; }

    public virtual DbSet<PersonStewardRegion> PersonStewardRegions { get; set; }

    public virtual DbSet<PersonStewardTaxonomyBranch> PersonStewardTaxonomyBranches { get; set; }

    public virtual DbSet<PriorityLandscape> PriorityLandscapes { get; set; }

    public virtual DbSet<PriorityLandscapeCategory> PriorityLandscapeCategories { get; set; }

    public virtual DbSet<PriorityLandscapeFileResource> PriorityLandscapeFileResources { get; set; }

    public virtual DbSet<Program> Programs { get; set; }

    public virtual DbSet<ProgramIndex> ProgramIndices { get; set; }

    public virtual DbSet<ProgramNotificationConfiguration> ProgramNotificationConfigurations { get; set; }

    public virtual DbSet<ProgramNotificationSent> ProgramNotificationSents { get; set; }

    public virtual DbSet<ProgramNotificationSentProject> ProgramNotificationSentProjects { get; set; }

    public virtual DbSet<ProgramPerson> ProgramPeople { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectClassification> ProjectClassifications { get; set; }

    public virtual DbSet<ProjectCode> ProjectCodes { get; set; }

    public virtual DbSet<ProjectCounty> ProjectCounties { get; set; }

    public virtual DbSet<ProjectCountyUpdate> ProjectCountyUpdates { get; set; }

    public virtual DbSet<ProjectDocument> ProjectDocuments { get; set; }

    public virtual DbSet<ProjectDocumentUpdate> ProjectDocumentUpdates { get; set; }

    public virtual DbSet<ProjectExternalLink> ProjectExternalLinks { get; set; }

    public virtual DbSet<ProjectExternalLinkUpdate> ProjectExternalLinkUpdates { get; set; }

    public virtual DbSet<ProjectFundSourceAllocationRequest> ProjectFundSourceAllocationRequests { get; set; }

    public virtual DbSet<ProjectFundSourceAllocationRequestUpdate> ProjectFundSourceAllocationRequestUpdates { get; set; }

    public virtual DbSet<ProjectFundingSource> ProjectFundingSources { get; set; }

    public virtual DbSet<ProjectFundingSourceUpdate> ProjectFundingSourceUpdates { get; set; }

    public virtual DbSet<ProjectImage> ProjectImages { get; set; }

    public virtual DbSet<ProjectImageUpdate> ProjectImageUpdates { get; set; }

    public virtual DbSet<ProjectImportBlockList> ProjectImportBlockLists { get; set; }

    public virtual DbSet<ProjectInternalNote> ProjectInternalNotes { get; set; }

    public virtual DbSet<ProjectLocation> ProjectLocations { get; set; }

    public virtual DbSet<ProjectLocationStaging> ProjectLocationStagings { get; set; }

    public virtual DbSet<ProjectLocationStagingUpdate> ProjectLocationStagingUpdates { get; set; }

    public virtual DbSet<ProjectLocationUpdate> ProjectLocationUpdates { get; set; }

    public virtual DbSet<ProjectNote> ProjectNotes { get; set; }

    public virtual DbSet<ProjectNoteUpdate> ProjectNoteUpdates { get; set; }

    public virtual DbSet<ProjectOrganization> ProjectOrganizations { get; set; }

    public virtual DbSet<ProjectOrganizationUpdate> ProjectOrganizationUpdates { get; set; }

    public virtual DbSet<ProjectPerson> ProjectPeople { get; set; }

    public virtual DbSet<ProjectPersonUpdate> ProjectPersonUpdates { get; set; }

    public virtual DbSet<ProjectPriorityLandscape> ProjectPriorityLandscapes { get; set; }

    public virtual DbSet<ProjectPriorityLandscapeUpdate> ProjectPriorityLandscapeUpdates { get; set; }

    public virtual DbSet<ProjectProgram> ProjectPrograms { get; set; }

    public virtual DbSet<ProjectRegion> ProjectRegions { get; set; }

    public virtual DbSet<ProjectRegionUpdate> ProjectRegionUpdates { get; set; }

    public virtual DbSet<ProjectTag> ProjectTags { get; set; }

    public virtual DbSet<ProjectType> ProjectTypes { get; set; }

    public virtual DbSet<ProjectUpdate> ProjectUpdates { get; set; }

    public virtual DbSet<ProjectUpdateBatch> ProjectUpdateBatches { get; set; }

    public virtual DbSet<ProjectUpdateConfiguration> ProjectUpdateConfigurations { get; set; }

    public virtual DbSet<ProjectUpdateHistory> ProjectUpdateHistories { get; set; }

    public virtual DbSet<ProjectUpdateProgram> ProjectUpdatePrograms { get; set; }

    public virtual DbSet<RelationshipType> RelationshipTypes { get; set; }

    public virtual DbSet<ReportTemplate> ReportTemplates { get; set; }

    public virtual DbSet<SocrataDataMartRawJsonImport> SocrataDataMartRawJsonImports { get; set; }

    public virtual DbSet<StateProvince> StateProvinces { get; set; }

    public virtual DbSet<SupportRequestLog> SupportRequestLogs { get; set; }

    public virtual DbSet<SystemAttribute> SystemAttributes { get; set; }

    public virtual DbSet<TabularDataImport> TabularDataImports { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TaxonomyBranch> TaxonomyBranches { get; set; }

    public virtual DbSet<TaxonomyLevel> TaxonomyLevels { get; set; }

    public virtual DbSet<TaxonomyTrunk> TaxonomyTrunks { get; set; }

    public virtual DbSet<TrainingVideo> TrainingVideos { get; set; }

    public virtual DbSet<Treatment> Treatments { get; set; }

    public virtual DbSet<TreatmentArea> TreatmentAreas { get; set; }

    public virtual DbSet<TreatmentUpdate> TreatmentUpdates { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public virtual DbSet<WashingtonLegislativeDistrict> WashingtonLegislativeDistricts { get; set; }

    public virtual DbSet<vArcOnlineRawJsonImportIndex> vArcOnlineRawJsonImportIndices { get; set; }

    public virtual DbSet<vGeoServerCounty> vGeoServerCounties { get; set; }

    public virtual DbSet<vGeoServerPriorityLandscape> vGeoServerPriorityLandscapes { get; set; }

    public virtual DbSet<vLoaStageFundSourceAllocation> vLoaStageFundSourceAllocations { get; set; }

    public virtual DbSet<vLoaStageFundSourceAllocationByProgramIndexProjectCode> vLoaStageFundSourceAllocationByProgramIndexProjectCodes { get; set; }

    public virtual DbSet<vLoaStageProjectFundSourceAllocation> vLoaStageProjectFundSourceAllocations { get; set; }

    public virtual DbSet<vSingularFundSourceAllocation> vSingularFundSourceAllocations { get; set; }

    public virtual DbSet<vSocrataDataMartRawJsonImportIndex> vSocrataDataMartRawJsonImportIndices { get; set; }

    public virtual DbSet<vTotalTreatedAcresByProject> vTotalTreatedAcresByProjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agreement>(entity =>
        {
            entity.HasKey(e => e.AgreementID).HasName("PK_Agreement_AgreementID");

            entity.HasOne(d => d.AgreementType).WithMany(p => p.Agreements).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Organization).WithMany(p => p.Agreements).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AgreementFundSourceAllocation>(entity =>
        {
            entity.HasKey(e => e.AgreementFundSourceAllocationID).HasName("PK_AgreementFundSourceAllocation_AgreementFundSourceAllocationID");

            entity.HasOne(d => d.Agreement).WithMany(p => p.AgreementFundSourceAllocations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.AgreementFundSourceAllocations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AgreementPerson>(entity =>
        {
            entity.HasKey(e => e.AgreementPersonID).HasName("PK_AgreementPerson_AgreementPersonID");

            entity.HasOne(d => d.Agreement).WithMany(p => p.AgreementPeople).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Person).WithMany(p => p.AgreementPeople).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AgreementProject>(entity =>
        {
            entity.HasKey(e => e.AgreementProjectID).HasName("PK_AgreementProject_AgreementProjectID");

            entity.HasOne(d => d.Agreement).WithMany(p => p.AgreementProjects).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.AgreementProjects).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AgreementStatus>(entity =>
        {
            entity.HasKey(e => e.AgreementStatusID).HasName("PK_AgreementStatus_AgreementStatusID");
        });

        modelBuilder.Entity<AgreementType>(entity =>
        {
            entity.HasKey(e => e.AgreementTypeID).HasName("PK_AgreementType_AgreementTypeID");
        });

        modelBuilder.Entity<ArcOnlineFinanceApiRawJsonImport>(entity =>
        {
            entity.HasKey(e => e.ArcOnlineFinanceApiRawJsonImportID).HasName("PK_ArcOnlineFinanceApiRawJsonImport_ArcOnlineFinanceApiRawJsonImportID");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogID).HasName("PK_AuditLog_AuditLogID");

            entity.HasOne(d => d.Person).WithMany(p => p.AuditLogs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Authenticator>(entity =>
        {
            entity.HasKey(e => e.AuthenticatorID).HasName("PK_Authenticator_AuthenticatorID");
        });

        modelBuilder.Entity<Classification>(entity =>
        {
            entity.HasKey(e => e.ClassificationID).HasName("PK_Classification_ClassificationID");

            entity.HasOne(d => d.ClassificationSystem).WithMany(p => p.Classifications).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.KeyImageFileResource).WithMany(p => p.Classifications).HasConstraintName("FK_Classification_FileResource_KeyImageFileResourceID_FileResourceID");
        });

        modelBuilder.Entity<ClassificationSystem>(entity =>
        {
            entity.HasKey(e => e.ClassificationSystemID).HasName("PK_ClassificationSystem_ClassificationSystemID");
        });

        modelBuilder.Entity<CostTypeDatamartMapping>(entity =>
        {
            entity.HasKey(e => e.CostTypeDatamartMappingID).HasName("PK_CostTypeDatamartMapping_CostTypeDatamartMappingID");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryID).HasName("PK_Country_CountryID");
        });

        modelBuilder.Entity<County>(entity =>
        {
            entity.HasKey(e => e.CountyID).HasName("PK_County_CountyID");

            entity.HasOne(d => d.StateProvince).WithMany(p => p.Counties).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CustomPage>(entity =>
        {
            entity.HasKey(e => e.CustomPageID).HasName("PK_CustomPage_CustomPageID");
        });

        modelBuilder.Entity<CustomPageImage>(entity =>
        {
            entity.HasKey(e => e.CustomPageImageID).HasName("PK_CustomPageImage_CustomPageImageID");

            entity.HasOne(d => d.CustomPage).WithMany(p => p.CustomPageImages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FileResource).WithMany(p => p.CustomPageImages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DNRUplandRegion>(entity =>
        {
            entity.HasKey(e => e.DNRUplandRegionID).HasName("PK_DNRUplandRegion_DNRUplandRegionID");

            entity.Property(e => e.DNRUplandRegionID).ValueGeneratedNever();

            entity.HasOne(d => d.DNRUplandRegionCoordinator).WithMany(p => p.DNRUplandRegions).HasConstraintName("FK_DNRUplandRegion_Person_DNRUplandRegionCoordinatorID_PersonID");
        });

        modelBuilder.Entity<DNRUplandRegionContentImage>(entity =>
        {
            entity.HasKey(e => e.DNRUplandRegionContentImageID).HasName("PK_DNRUplandRegionContentImage_DNRUplandRegionContentImageID");

            entity.HasOne(d => d.DNRUplandRegion).WithMany(p => p.DNRUplandRegionContentImages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FileResource).WithMany(p => p.DNRUplandRegionContentImages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ExternalMapLayer>(entity =>
        {
            entity.HasKey(e => e.ExternalMapLayerID).HasName("PK_ExternalMapLayer_ExternalMapLayerID");
        });

        modelBuilder.Entity<FederalFundCode>(entity =>
        {
            entity.HasKey(e => e.FederalFundCodeID).HasName("PK_FederalFundCode_FederalFundCodeID");
        });

        modelBuilder.Entity<FieldDefinitionDatum>(entity =>
        {
            entity.HasKey(e => e.FieldDefinitionDatumID).HasName("PK_FieldDefinitionDatum_FieldDefinitionDatumID");
        });

        modelBuilder.Entity<FieldDefinitionDatumImage>(entity =>
        {
            entity.HasKey(e => e.FieldDefinitionDatumImageID).HasName("PK_FieldDefinitionDatumImage_FieldDefinitionDatumImageID");

            entity.HasOne(d => d.FieldDefinitionDatum).WithMany(p => p.FieldDefinitionDatumImages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FileResource).WithMany(p => p.FieldDefinitionDatumImages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FileResource>(entity =>
        {
            entity.HasKey(e => e.FileResourceID).HasName("PK_FileResource_FileResourceID");

            entity.HasOne(d => d.CreatePerson).WithMany(p => p.FileResources)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FileResource_Person_CreatePersonID_PersonID");
        });

        modelBuilder.Entity<FileResourceMimeTypeFileExtension>(entity =>
        {
            entity.HasKey(e => e.FileResourceMimeTypeFileExtensionID).HasName("PK_FileResourceMimeTypeFileExtension_FileResourceMimeTypeFileExtensionID");
        });

        modelBuilder.Entity<FindYourForesterQuestion>(entity =>
        {
            entity.HasKey(e => e.FindYourForesterQuestionID).HasName("PK_FindYourForesterQuestion_FindYourForesterQuestionID");

            entity.HasOne(d => d.ParentQuestion).WithMany(p => p.InverseParentQuestion).HasConstraintName("FK_FindYourForesterQuestion_FindYourForesterQuestion_ParentQuestionID_FindYourForesterQuestionID");
        });

        modelBuilder.Entity<FirmaHomePageImage>(entity =>
        {
            entity.HasKey(e => e.FirmaHomePageImageID).HasName("PK_FirmaHomePageImage_FirmaHomePageImageID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.FirmaHomePageImages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FirmaPage>(entity =>
        {
            entity.HasKey(e => e.FirmaPageID).HasName("PK_FirmaPage_FirmaPageID");
        });

        modelBuilder.Entity<FirmaPageImage>(entity =>
        {
            entity.HasKey(e => e.FirmaPageImageID).HasName("PK_FirmaPageImage_FirmaPageImageID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.FirmaPageImages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FirmaPage).WithMany(p => p.FirmaPageImages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FocusArea>(entity =>
        {
            entity.HasKey(e => e.FocusAreaID).HasName("PK_FocusArea_FocusAreaID");

            entity.HasOne(d => d.DNRUplandRegion).WithMany(p => p.FocusAreas).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FocusAreaLocationStaging>(entity =>
        {
            entity.HasKey(e => e.FocusAreaLocationStagingID).HasName("PK_FocusAreaLocationStaging_FocusAreaLocationStagingID");

            entity.HasOne(d => d.FocusArea).WithMany(p => p.FocusAreaLocationStagings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ForesterWorkUnit>(entity =>
        {
            entity.HasKey(e => e.ForesterWorkUnitID).HasName("PK_ForesterWorkUnit_ForesterWorkUnitID");
        });

        modelBuilder.Entity<FundSource>(entity =>
        {
            entity.HasKey(e => e.FundSourceID).HasName("PK_FundSource_FundSourceID");

            entity.HasOne(d => d.Organization).WithMany(p => p.FundSources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceAllocation>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationID).HasName("PK_FundSourceAllocation_FundSourceAllocationID");

            entity.HasOne(d => d.FundSource).WithMany(p => p.FundSourceAllocations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FundSourceManager).WithMany(p => p.FundSourceAllocations).HasConstraintName("FK_FundSourceAllocation_Person_FundSourceManagerID_PersonID");
        });

        modelBuilder.Entity<FundSourceAllocationBudgetLineItem>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationBudgetLineItemID).HasName("PK_FundSourceAllocationBudgetLineItem_FundSourceAllocationBudgetLineItemID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationBudgetLineItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceAllocationChangeLog>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationChangeLogID).HasName("PK_FundSourceAllocationChangeLog_FundSourceAllocationChangeLogID");

            entity.HasOne(d => d.ChangePerson).WithMany(p => p.FundSourceAllocationChangeLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FundSourceAllocationChangeLog_Person_ChangePersonID_PersonID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationChangeLogs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceAllocationExpenditure>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationExpenditureID).HasName("PK_FundSourceAllocationExpenditure_FundSourceAllocationExpenditureID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationExpenditures).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceAllocationExpenditureJsonStage>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationExpenditureJsonStageID).HasName("PK_FundSourceAllocationExpenditureJsonStage_FundSourceAllocationExpenditureJsonStageID");
        });

        modelBuilder.Entity<FundSourceAllocationFileResource>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationFileResourceID).HasName("PK_FundSourceAllocationFileResource_FundSourceAllocationFileResourceID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.FundSourceAllocationFileResources).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationFileResources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceAllocationLikelyPerson>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationLikelyPersonID).HasName("PK_FundSourceAllocationLikelyPerson_FundSourceAllocationLikelyPersonID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationLikelyPeople).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Person).WithMany(p => p.FundSourceAllocationLikelyPeople).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceAllocationNote>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationNoteID).HasName("PK_FundSourceAllocationNote_FundSourceAllocationNoteID");

            entity.HasOne(d => d.CreatedByPerson).WithMany(p => p.FundSourceAllocationNoteCreatedByPeople)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FundSourceAllocationNote_Person_CreatedByPersonID_PersonID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationNotes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LastUpdatedByPerson).WithMany(p => p.FundSourceAllocationNoteLastUpdatedByPeople).HasConstraintName("FK_FundSourceAllocationNote_Person_LastUpdatedByPersonID_PersonID");
        });

        modelBuilder.Entity<FundSourceAllocationNoteInternal>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationNoteInternalID).HasName("PK_FundSourceAllocationNoteInternal_FundSourceAllocationNoteInternalID");

            entity.HasOne(d => d.CreatedByPerson).WithMany(p => p.FundSourceAllocationNoteInternalCreatedByPeople)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FundSourceAllocationNoteInternal_Person_CreatedByPersonID_PersonID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationNoteInternals).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LastUpdatedByPerson).WithMany(p => p.FundSourceAllocationNoteInternalLastUpdatedByPeople).HasConstraintName("FK_FundSourceAllocationNoteInternal_Person_LastUpdatedByPersonID_PersonID");
        });

        modelBuilder.Entity<FundSourceAllocationPriority>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationPriorityID).HasName("PK_FundSourceAllocationPriority_FundSourceAllocationPriorityID");

            entity.Property(e => e.FundSourceAllocationPriorityID).ValueGeneratedNever();
        });

        modelBuilder.Entity<FundSourceAllocationProgramIndexProjectCode>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationProgramIndexProjectCodeID).HasName("PK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocationProgramIndexProjectCodeID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationProgramIndexProjectCodes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProgramIndex).WithMany(p => p.FundSourceAllocationProgramIndexProjectCodes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceAllocationProgramManager>(entity =>
        {
            entity.HasKey(e => e.FundSourceAllocationProgramManagerID).HasName("PK_FundSourceAllocationProgramManager_FundSourceAllocationProgramManagerID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.FundSourceAllocationProgramManagers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Person).WithMany(p => p.FundSourceAllocationProgramManagers).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceFileResource>(entity =>
        {
            entity.HasKey(e => e.FundSourceFileResourceID).HasName("PK_FundSourceFileResource_FundSourceFileResourceID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.FundSourceFileResources).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FundSource).WithMany(p => p.FundSourceFileResources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FundSourceNote>(entity =>
        {
            entity.HasKey(e => e.FundSourceNoteID).HasName("PK_FundSourceNote_FundSourceNoteID");

            entity.HasOne(d => d.CreatedByPerson).WithMany(p => p.FundSourceNoteCreatedByPeople)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FundSourceNote_Person_CreatedByPersonID_PersonID");

            entity.HasOne(d => d.FundSource).WithMany(p => p.FundSourceNotes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LastUpdatedByPerson).WithMany(p => p.FundSourceNoteLastUpdatedByPeople).HasConstraintName("FK_FundSourceNote_Person_LastUpdatedByPersonID_PersonID");
        });

        modelBuilder.Entity<FundSourceNoteInternal>(entity =>
        {
            entity.HasKey(e => e.FundSourceNoteInternalID).HasName("PK_FundSourceNoteInternal_FundSourceNoteInternalID");

            entity.HasOne(d => d.CreatedByPerson).WithMany(p => p.FundSourceNoteInternalCreatedByPeople)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FundSourceNoteInternal_Person_CreatedByPersonID_PersonID");

            entity.HasOne(d => d.FundSource).WithMany(p => p.FundSourceNoteInternals).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LastUpdatedByPerson).WithMany(p => p.FundSourceNoteInternalLastUpdatedByPeople).HasConstraintName("FK_FundSourceNoteInternal_Person_LastUpdatedByPersonID_PersonID");
        });

        modelBuilder.Entity<FundSourceType>(entity =>
        {
            entity.HasKey(e => e.FundSourceTypeID).HasName("PK_FundSourceType_FundSourceTypeID");
        });

        modelBuilder.Entity<GisCrossWalkDefault>(entity =>
        {
            entity.HasKey(e => e.GisCrossWalkDefaultID).HasName("PK_GisCrossWalkDefault_GisCrossWalkDefaultID");

            entity.HasOne(d => d.GisUploadSourceOrganization).WithMany(p => p.GisCrossWalkDefaults).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GisDefaultMapping>(entity =>
        {
            entity.HasKey(e => e.GisDefaultMappingID).HasName("PK_GisDefaultMapping_GisDefaultMappingID");

            entity.HasOne(d => d.GisUploadSourceOrganization).WithMany(p => p.GisDefaultMappings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GisExcludeIncludeColumn>(entity =>
        {
            entity.HasKey(e => e.GisExcludeIncludeColumnID).HasName("PK_GisExcludeIncludeColumn_GisExcludeIncludeColumnID");

            entity.Property(e => e.IsBlacklist).HasComputedColumnSql("(CONVERT([bit],case when [IsWhitelist]=(1) then (0) else (1) end))", false);

            entity.HasOne(d => d.GisUploadSourceOrganization).WithMany(p => p.GisExcludeIncludeColumns).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GisExcludeIncludeColumnValue>(entity =>
        {
            entity.HasKey(e => e.GisExcludeIncludeColumnValueID).HasName("PK_GisExcludeIncludeColumnValue_GisExcludeIncludeColumnValueID");

            entity.HasOne(d => d.GisExcludeIncludeColumn).WithMany(p => p.GisExcludeIncludeColumnValues).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GisFeature>(entity =>
        {
            entity.HasKey(e => e.GisFeatureID).HasName("PK_GisFeature_GisFeatureID");

            entity.Property(e => e.IsValid).HasComputedColumnSql("([GisFeatureGeometry].[STIsValid]())", false);

            entity.HasOne(d => d.GisUploadAttempt).WithMany(p => p.GisFeatures).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GisFeatureMetadataAttribute>(entity =>
        {
            entity.HasKey(e => e.GisFeatureMetadataAttributeID).HasName("PK_GisFeatureMetadataAttribute_GisFeatureMetadataAttributeID");

            entity.HasOne(d => d.GisFeature).WithMany(p => p.GisFeatureMetadataAttributes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.GisMetadataAttribute).WithMany(p => p.GisFeatureMetadataAttributes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GisMetadataAttribute>(entity =>
        {
            entity.HasKey(e => e.GisMetadataAttributeID).HasName("PK_GisMetadataAttribute_GisMetadataAttributeID");
        });

        modelBuilder.Entity<GisUploadAttempt>(entity =>
        {
            entity.HasKey(e => e.GisUploadAttemptID).HasName("PK_GisUploadAttempt_GisUploadAttemptID");

            entity.HasOne(d => d.GisUploadAttemptCreatePerson).WithMany(p => p.GisUploadAttempts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GisUploadAttempt_Person_GisUploadAttemptCreatePersonID_PersonID");

            entity.HasOne(d => d.GisUploadSourceOrganization).WithMany(p => p.GisUploadAttempts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GisUploadAttemptGisMetadataAttribute>(entity =>
        {
            entity.HasKey(e => e.GisUploadAttemptGisMetadataAttributeID).HasName("PK_GisUploadAttemptGisMetadataAttribute_GisUploadAttemptGisMetadataAttributeID");

            entity.HasOne(d => d.GisMetadataAttribute).WithMany(p => p.GisUploadAttemptGisMetadataAttributes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.GisUploadAttempt).WithMany(p => p.GisUploadAttemptGisMetadataAttributes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GisUploadProgramMergeGrouping>(entity =>
        {
            entity.HasKey(e => e.GisUploadProgramMergeGroupingID).HasName("PK_GisUploadProgramMergeGrouping_GisUploadProgramMergeGroupingID");
        });

        modelBuilder.Entity<GisUploadSourceOrganization>(entity =>
        {
            entity.HasKey(e => e.GisUploadSourceOrganizationID).HasName("PK_GisUploadSourceOrganization_GisUploadSourceOrganizationID");

            entity.HasOne(d => d.DefaultLeadImplementerOrganization).WithMany(p => p.GisUploadSourceOrganizations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GisUploadSourceOrganization_Organization_DefaultLeadImplementerOrganizationID_OrganizationID");

            entity.HasOne(d => d.Program).WithOne(p => p.GisUploadSourceOrganization).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RelationshipTypeForDefaultOrganization).WithMany(p => p.GisUploadSourceOrganizations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GisUploadSourceOrganization_RelationshipType_RelationshipTypeForDefaultOrganizationID_RelationshipTypeID");
        });

        modelBuilder.Entity<InteractionEvent>(entity =>
        {
            entity.HasKey(e => e.InteractionEventID).HasName("PK_InteractionEvent_InteractionEventID");

            entity.HasOne(d => d.StaffPerson).WithMany(p => p.InteractionEvents).HasConstraintName("FK_InteractionEvent_Person_StaffPersonID_PersonID");
        });

        modelBuilder.Entity<InteractionEventContact>(entity =>
        {
            entity.HasKey(e => e.InteractionEventContactID).HasName("PK_InteractionEventContact_InteractionEventContactID");

            entity.HasOne(d => d.InteractionEvent).WithMany(p => p.InteractionEventContacts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Person).WithMany(p => p.InteractionEventContacts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InteractionEventFileResource>(entity =>
        {
            entity.HasKey(e => e.InteractionEventFileResourceID).HasName("PK_InteractionEventFileResource_InteractionEventFileResourceID");

            entity.HasOne(d => d.InteractionEvent).WithMany(p => p.InteractionEventFileResources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InteractionEventProject>(entity =>
        {
            entity.HasKey(e => e.InteractionEventProjectID).HasName("PK_InteractionEventProject_InteractionEventProjectID");

            entity.HasOne(d => d.InteractionEvent).WithMany(p => p.InteractionEventProjects).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.InteractionEventProjects).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceID).HasName("PK_Invoice_InvoiceID");

            entity.HasOne(d => d.InvoiceApprovalStatus).WithMany(p => p.Invoices).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InvoiceFileResource).WithMany(p => p.Invoices).HasConstraintName("FK_Invoice_FileResource_InvoiceFileResourceID_FileResourceID");

            entity.HasOne(d => d.InvoicePaymentRequest).WithMany(p => p.Invoices).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InvoiceApprovalStatus>(entity =>
        {
            entity.HasKey(e => e.InvoiceApprovalStatusID).HasName("PK_InvoiceApprovalStatus_InvoiceApprovalStatusID");

            entity.Property(e => e.InvoiceApprovalStatusID).ValueGeneratedNever();
        });

        modelBuilder.Entity<InvoicePaymentRequest>(entity =>
        {
            entity.HasKey(e => e.InvoicePaymentRequestID).HasName("PK_InvoicePaymentRequest_InvoicePaymentRequestID");

            entity.HasOne(d => d.PreparedByPerson).WithMany(p => p.InvoicePaymentRequests).HasConstraintName("FK_InvoicePaymentRequest_Person_PreparedByPersonID_PersonID");

            entity.HasOne(d => d.Project).WithMany(p => p.InvoicePaymentRequests).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<LoaStage>(entity =>
        {
            entity.HasKey(e => e.LoaStageID).HasName("PK_LoaStage_LoaStageID");

            entity.Property(e => e.IsSoutheast).HasComputedColumnSql("(CONVERT([bit],case when [IsNortheast]=(1) then (0) else (1) end))", true);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationID).HasName("PK_Notification_NotificationID");

            entity.HasOne(d => d.Person).WithMany(p => p.Notifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<NotificationProject>(entity =>
        {
            entity.HasKey(e => e.NotificationProjectID).HasName("PK_NotificationProject_NotificationProjectID");

            entity.HasOne(d => d.Notification).WithMany(p => p.NotificationProjects).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.NotificationProjects).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationID).HasName("PK_Organization_OrganizationID");

            entity.HasOne(d => d.LogoFileResource).WithMany(p => p.Organizations).HasConstraintName("FK_Organization_FileResource_LogoFileResourceID_FileResourceID");

            entity.HasOne(d => d.OrganizationType).WithMany(p => p.Organizations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PrimaryContactPerson).WithMany(p => p.Organizations).HasConstraintName("FK_Organization_Person_PrimaryContactPersonID_PersonID");
        });

        modelBuilder.Entity<OrganizationBoundaryStaging>(entity =>
        {
            entity.HasKey(e => e.OrganizationBoundaryStagingID).HasName("PK_OrganizationBoundaryStaging_OrganizationBoundaryStagingID");

            entity.HasOne(d => d.Organization).WithMany(p => p.OrganizationBoundaryStagings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationType>(entity =>
        {
            entity.HasKey(e => e.OrganizationTypeID).HasName("PK_OrganizationType_OrganizationTypeID");
        });

        modelBuilder.Entity<OrganizationTypeRelationshipType>(entity =>
        {
            entity.HasKey(e => e.OrganizationTypeRelationshipTypeID).HasName("PK_OrganizationTypeRelationshipType_OrganizationTypeRelationshipTypeID");

            entity.HasOne(d => d.OrganizationType).WithMany(p => p.OrganizationTypeRelationshipTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RelationshipType).WithMany(p => p.OrganizationTypeRelationshipTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonID).HasName("PK_Person_PersonID");

            entity.HasIndex(e => e.Email, "IX_Person_Email_UniqueWhenNotNull")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL)");

            entity.HasOne(d => d.AddedByPerson).WithMany(p => p.InverseAddedByPerson).HasConstraintName("FK_Person_Person_AddedByPersonID_PersonID");

            entity.HasOne(d => d.ImpersonatedPerson).WithMany(p => p.InverseImpersonatedPerson).HasConstraintName("FK_Person_Person_ImpersonatedPersonID_PersonID");
        });

        modelBuilder.Entity<PersonAllowedAuthenticator>(entity =>
        {
            entity.HasKey(e => e.PersonAllowedAuthenticatorID)
                .HasName("PK_PersonAllowedAuthenticator_PersonAllowedAuthenticatorID")
                .IsClustered(false);

            entity.HasIndex(e => new { e.PersonID, e.AuthenticatorID }, "AK_PersonAllowedAuthenticator_PersonID_AuthenticatorID")
                .IsUnique()
                .IsClustered();

            entity.HasOne(d => d.Authenticator).WithMany(p => p.PersonAllowedAuthenticators).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Person).WithMany(p => p.PersonAllowedAuthenticators).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonRole>(entity =>
        {
            entity.HasKey(e => e.PersonRoleID).HasName("PK_PersonRole_PersonRoleID");

            entity.HasOne(d => d.Person).WithMany(p => p.PersonRoles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonStewardOrganization>(entity =>
        {
            entity.HasKey(e => e.PersonStewardOrganizationID).HasName("PK_PersonStewardOrganization_PersonStewardOrganizationID");

            entity.HasOne(d => d.Organization).WithMany(p => p.PersonStewardOrganizations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Person).WithMany(p => p.PersonStewardOrganizations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonStewardRegion>(entity =>
        {
            entity.HasKey(e => e.PersonStewardRegionID).HasName("PK_PersonStewardRegion_PersonStewardRegionID");

            entity.HasOne(d => d.DNRUplandRegion).WithMany(p => p.PersonStewardRegions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Person).WithMany(p => p.PersonStewardRegions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonStewardTaxonomyBranch>(entity =>
        {
            entity.HasKey(e => e.PersonStewardTaxonomyBranchID).HasName("PK_PersonStewardTaxonomyBranch_PersonStewardTaxonomyBranchID");

            entity.HasOne(d => d.Person).WithMany(p => p.PersonStewardTaxonomyBranches).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TaxonomyBranch).WithMany(p => p.PersonStewardTaxonomyBranches).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PriorityLandscape>(entity =>
        {
            entity.HasKey(e => e.PriorityLandscapeID).HasName("PK_PriorityLandscape_PriorityLandscapeID");

            entity.Property(e => e.PriorityLandscapeID).ValueGeneratedNever();
        });

        modelBuilder.Entity<PriorityLandscapeCategory>(entity =>
        {
            entity.HasKey(e => e.PriorityLandscapeCategoryID).HasName("PK_PriorityLandscapeCategory_PriorityLandscapeCategoryID");

            entity.Property(e => e.PriorityLandscapeCategoryID).ValueGeneratedNever();
        });

        modelBuilder.Entity<PriorityLandscapeFileResource>(entity =>
        {
            entity.HasKey(e => e.PriorityLandscapeFileResourceID).HasName("PK_PriorityLandscapeFileResource_PriorityLandscapeFileResourceID");

            entity.HasOne(d => d.PriorityLandscape).WithMany(p => p.PriorityLandscapeFileResources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Program>(entity =>
        {
            entity.HasKey(e => e.ProgramID).HasName("PK_Program_ProgramID");

            entity.HasOne(d => d.Organization).WithMany(p => p.Programs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProgramCreatePerson).WithMany(p => p.ProgramProgramCreatePeople)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Program_Person_ProgramCreatePersonID_PersonID");

            entity.HasOne(d => d.ProgramExampleGeospatialUploadFileResource).WithMany(p => p.ProgramProgramExampleGeospatialUploadFileResources).HasConstraintName("FK_Program_FileResource_ProgramExampleGeospatialUploadFileResourceID_FileResourceID");

            entity.HasOne(d => d.ProgramFileResource).WithMany(p => p.ProgramProgramFileResources).HasConstraintName("FK_Program_FileResource_ProgramFileResourceID_FileResourceID");

            entity.HasOne(d => d.ProgramLastUpdatedByPerson).WithMany(p => p.ProgramProgramLastUpdatedByPeople).HasConstraintName("FK_Program_Person_ProgramLastUpdatedByPersonID_PersonID");

            entity.HasOne(d => d.ProgramPrimaryContactPerson).WithMany(p => p.ProgramProgramPrimaryContactPeople).HasConstraintName("FK_Program_Person_ProgramPrimaryContactPersonID_PersonID");
        });

        modelBuilder.Entity<ProgramIndex>(entity =>
        {
            entity.HasKey(e => e.ProgramIndexID).HasName("PK_ProgramIndex_ProgramIndexID");
        });

        modelBuilder.Entity<ProgramNotificationConfiguration>(entity =>
        {
            entity.HasKey(e => e.ProgramNotificationConfigurationID).HasName("PK_ProgramNotificationConfiguration_ProgramNotificationConfigurationID");

            entity.HasOne(d => d.Program).WithMany(p => p.ProgramNotificationConfigurations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProgramNotificationSent>(entity =>
        {
            entity.HasKey(e => e.ProgramNotificationSentID).HasName("PK_ProgramNotificationSent_ProgramNotificationSentID");

            entity.HasOne(d => d.ProgramNotificationConfiguration).WithMany(p => p.ProgramNotificationSents).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SentToPerson).WithMany(p => p.ProgramNotificationSents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProgramNotificationSent_Person_SentToPersonID_PersonID");
        });

        modelBuilder.Entity<ProgramNotificationSentProject>(entity =>
        {
            entity.HasKey(e => e.ProgramNotificationSentProjectID).HasName("PK_ProgramNotificationSentProject_ProgramNotificationSentProjectID");

            entity.HasOne(d => d.ProgramNotificationSent).WithMany(p => p.ProgramNotificationSentProjects).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProgramNotificationSentProjects).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProgramPerson>(entity =>
        {
            entity.HasKey(e => e.ProgramPersonID).HasName("PK_ProgramPerson_ProgramPersonID");

            entity.HasOne(d => d.Person).WithMany(p => p.ProgramPeople).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Program).WithMany(p => p.ProgramPeople).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectID).HasName("PK_Project_ProjectID");

            entity.HasOne(d => d.CreateGisUploadAttempt).WithMany(p => p.ProjectCreateGisUploadAttempts).HasConstraintName("FK_Project_GisUploadAttempt_CreateGisUploadAttemptID_GisUploadAttemptID");

            entity.HasOne(d => d.LastUpdateGisUploadAttempt).WithMany(p => p.ProjectLastUpdateGisUploadAttempts).HasConstraintName("FK_Project_GisUploadAttempt_LastUpdateGisUploadAttemptID_GisUploadAttemptID");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.Projects).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProposingPerson).WithMany(p => p.ProjectProposingPeople).HasConstraintName("FK_Project_Person_ProposingPersonID_PersonID");

            entity.HasOne(d => d.ReviewedByPerson).WithMany(p => p.ProjectReviewedByPeople).HasConstraintName("FK_Project_Person_ReviewedByPersonID_PersonID");
        });

        modelBuilder.Entity<ProjectClassification>(entity =>
        {
            entity.HasKey(e => e.ProjectClassificationID).HasName("PK_ProjectClassification_ProjectClassificationID");

            entity.HasOne(d => d.Classification).WithMany(p => p.ProjectClassifications).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectClassifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectCode>(entity =>
        {
            entity.HasKey(e => e.ProjectCodeID).HasName("PK_ProjectCode_ProjectCodeID");
        });

        modelBuilder.Entity<ProjectCounty>(entity =>
        {
            entity.HasKey(e => e.ProjectCountyID).HasName("PK_ProjectCounty_ProjectCountyID");

            entity.HasOne(d => d.County).WithMany(p => p.ProjectCounties).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectCounties).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectCountyUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectCountyUpdateID).HasName("PK_ProjectCountyUpdate_ProjectCountyUpdateID");

            entity.HasOne(d => d.County).WithMany(p => p.ProjectCountyUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectCountyUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectDocument>(entity =>
        {
            entity.HasKey(e => e.ProjectDocumentID).HasName("PK_ProjectDocument_ProjectDocumentID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.ProjectDocuments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectDocuments).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectDocumentUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectDocumentUpdateID).HasName("PK_ProjectDocumentUpdate_ProjectDocumentUpdateID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.ProjectDocumentUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectDocumentUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectExternalLink>(entity =>
        {
            entity.HasKey(e => e.ProjectExternalLinkID).HasName("PK_ProjectExternalLink_ProjectExternalLinkID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectExternalLinks).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectExternalLinkUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectExternalLinkUpdateID).HasName("PK_ProjectExternalLinkUpdate_ProjectExternalLinkUpdateID");

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectExternalLinkUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectFundSourceAllocationRequest>(entity =>
        {
            entity.HasKey(e => e.ProjectFundSourceAllocationRequestID).HasName("PK_ProjectFundSourceAllocationRequest_ProjectFundSourceAllocationRequestID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.ProjectFundSourceAllocationRequests).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectFundSourceAllocationRequests).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectFundSourceAllocationRequestUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectFundSourceAllocationRequestUpdateID).HasName("PK_ProjectFundSourceAllocationRequestUpdate_ProjectFundSourceAllocationRequestUpdateID");

            entity.HasOne(d => d.FundSourceAllocation).WithMany(p => p.ProjectFundSourceAllocationRequestUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectFundSourceAllocationRequestUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectFundingSource>(entity =>
        {
            entity.HasKey(e => e.ProjectFundingSourceID).HasName("PK_ProjectFundingSource_ProjectFundingSourceID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectFundingSources).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectFundingSourceUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectFundingSourceUpdateID).HasName("PK_ProjectFundingSourceUpdate_ProjectFundingSourceUpdateID");

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectFundingSourceUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectImage>(entity =>
        {
            entity.HasKey(e => e.ProjectImageID).HasName("PK_ProjectImage_ProjectImageID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.ProjectImages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectImages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectImageUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectImageUpdateID).HasName("PK_ProjectImageUpdate_ProjectImageUpdateID");

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectImageUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectImportBlockList>(entity =>
        {
            entity.HasKey(e => e.ProjectImportBlockListID).HasName("PK_ProjectImportBlockList_ProjectImportBlockListID");

            entity.HasOne(d => d.Program).WithMany(p => p.ProjectImportBlockLists).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectInternalNote>(entity =>
        {
            entity.HasKey(e => e.ProjectInternalNoteID).HasName("PK_ProjectInternalNote_ProjectInternalNoteID");

            entity.HasOne(d => d.CreatePerson).WithMany(p => p.ProjectInternalNoteCreatePeople).HasConstraintName("FK_ProjectInternalNote_Person_CreatePersonID_PersonID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectInternalNotes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatePerson).WithMany(p => p.ProjectInternalNoteUpdatePeople).HasConstraintName("FK_ProjectInternalNote_Person_UpdatePersonID_PersonID");
        });

        modelBuilder.Entity<ProjectLocation>(entity =>
        {
            entity.HasKey(e => e.ProjectLocationID).HasName("PK_ProjectLocation_ProjectLocationID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectLocations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectLocationStaging>(entity =>
        {
            entity.HasKey(e => e.ProjectLocationStagingID).HasName("PK_ProjectLocationStaging_ProjectLocationStagingID");

            entity.HasOne(d => d.Person).WithMany(p => p.ProjectLocationStagings).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectLocationStagings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectLocationStagingUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectLocationStagingUpdateID).HasName("PK_ProjectLocationStagingUpdate_ProjectLocationStagingUpdateID");

            entity.HasOne(d => d.Person).WithMany(p => p.ProjectLocationStagingUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectLocationStagingUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectLocationUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectLocationUpdateID).HasName("PK_ProjectLocationUpdate_ProjectLocationUpdateID");

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectLocationUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectNote>(entity =>
        {
            entity.HasKey(e => e.ProjectNoteID).HasName("PK_ProjectNote_ProjectNoteID");

            entity.HasOne(d => d.CreatePerson).WithMany(p => p.ProjectNoteCreatePeople).HasConstraintName("FK_ProjectNote_Person_CreatePersonID_PersonID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectNotes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatePerson).WithMany(p => p.ProjectNoteUpdatePeople).HasConstraintName("FK_ProjectNote_Person_UpdatePersonID_PersonID");
        });

        modelBuilder.Entity<ProjectNoteUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectNoteUpdateID).HasName("PK_ProjectNoteUpdate_ProjectNoteUpdateID");

            entity.HasOne(d => d.CreatePerson).WithMany(p => p.ProjectNoteUpdateCreatePeople).HasConstraintName("FK_ProjectNoteUpdate_Person_CreatePersonID_PersonID");

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectNoteUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatePerson).WithMany(p => p.ProjectNoteUpdateUpdatePeople).HasConstraintName("FK_ProjectNoteUpdate_Person_UpdatePersonID_PersonID");
        });

        modelBuilder.Entity<ProjectOrganization>(entity =>
        {
            entity.HasKey(e => e.ProjectOrganizationID).HasName("PK_ProjectOrganization_ProjectOrganizationID");

            entity.HasOne(d => d.Organization).WithMany(p => p.ProjectOrganizations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectOrganizations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RelationshipType).WithMany(p => p.ProjectOrganizations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectOrganizationUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectOrganizationUpdateID).HasName("PK_ProjectOrganizationUpdate_ProjectOrganizationUpdateID");

            entity.HasOne(d => d.Organization).WithMany(p => p.ProjectOrganizationUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectOrganizationUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RelationshipType).WithMany(p => p.ProjectOrganizationUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectPerson>(entity =>
        {
            entity.HasKey(e => e.ProjectPersonID).HasName("PK_ProjectPerson_ProjectPersonID");

            entity.HasOne(d => d.Person).WithMany(p => p.ProjectPeople).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectPeople).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectPersonUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectPersonUpdateID).HasName("PK_ProjectPersonUpdate_ProjectPersonUpdateID");

            entity.HasOne(d => d.Person).WithMany(p => p.ProjectPersonUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectPersonUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectPriorityLandscape>(entity =>
        {
            entity.HasKey(e => e.ProjectPriorityLandscapeID).HasName("PK_ProjectPriorityLandscape_ProjectPriorityLandscapeID");

            entity.HasOne(d => d.PriorityLandscape).WithMany(p => p.ProjectPriorityLandscapes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectPriorityLandscapes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectPriorityLandscapeUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectPriorityLandscapeUpdateID).HasName("PK_ProjectPriorityLandscapeUpdate_ProjectPriorityLandscapeUpdateID");

            entity.HasOne(d => d.PriorityLandscape).WithMany(p => p.ProjectPriorityLandscapeUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectPriorityLandscapeUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectProgram>(entity =>
        {
            entity.HasKey(e => e.ProjectProgramID).HasName("PK_ProjectProgram_ProjectProgramID");

            entity.HasOne(d => d.Program).WithMany(p => p.ProjectPrograms).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectPrograms).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectRegion>(entity =>
        {
            entity.HasKey(e => e.ProjectRegionID).HasName("PK_ProjectRegion_ProjectRegionID");

            entity.HasOne(d => d.DNRUplandRegion).WithMany(p => p.ProjectRegions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectRegions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectRegionUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectRegionUpdateID).HasName("PK_ProjectRegionUpdate_ProjectRegionUpdateID");

            entity.HasOne(d => d.DNRUplandRegion).WithMany(p => p.ProjectRegionUpdates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectRegionUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectTag>(entity =>
        {
            entity.HasKey(e => e.ProjectTagID).HasName("PK_ProjectTag_ProjectTagID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTags).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Tag).WithMany(p => p.ProjectTags).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectType>(entity =>
        {
            entity.HasKey(e => e.ProjectTypeID).HasName("PK_ProjectType_ProjectTypeID");

            entity.HasOne(d => d.TaxonomyBranch).WithMany(p => p.ProjectTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectUpdate>(entity =>
        {
            entity.HasKey(e => e.ProjectUpdateID).HasName("PK_ProjectUpdate_ProjectUpdateID");

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectUpdateBatch>(entity =>
        {
            entity.HasKey(e => e.ProjectUpdateBatchID).HasName("PK_ProjectUpdateBatch_ProjectUpdateBatchID");

            entity.HasOne(d => d.LastUpdatePerson).WithMany(p => p.ProjectUpdateBatches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectUpdateBatch_Person_LastUpdatePersonID_PersonID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectUpdateBatches).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectUpdateConfiguration>(entity =>
        {
            entity.HasKey(e => e.ProjectUpdateConfigurationID).HasName("PK_ProjectUpdateConfiguration_ProjectUpdateConfigurationID");
        });

        modelBuilder.Entity<ProjectUpdateHistory>(entity =>
        {
            entity.HasKey(e => e.ProjectUpdateHistoryID).HasName("PK_ProjectUpdateHistory_ProjectUpdateHistoryID");

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectUpdateHistories).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatePerson).WithMany(p => p.ProjectUpdateHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectUpdateHistory_Person_UpdatePersonID_PersonID");
        });

        modelBuilder.Entity<ProjectUpdateProgram>(entity =>
        {
            entity.HasKey(e => e.ProjectUpdateProgramID).HasName("PK_ProjectUpdateProgram_ProjectUpdateProgramID");

            entity.HasOne(d => d.Program).WithMany(p => p.ProjectUpdatePrograms).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.ProjectUpdatePrograms).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RelationshipType>(entity =>
        {
            entity.HasKey(e => e.RelationshipTypeID).HasName("PK_RelationshipType_RelationshipTypeID");
        });

        modelBuilder.Entity<ReportTemplate>(entity =>
        {
            entity.HasKey(e => e.ReportTemplateID).HasName("PK_ReportTemplate_ReportTemplateID");

            entity.HasOne(d => d.FileResource).WithMany(p => p.ReportTemplates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SocrataDataMartRawJsonImport>(entity =>
        {
            entity.HasKey(e => e.SocrataDataMartRawJsonImportID).HasName("PK_SocrataDataMartRawJsonImport_SocrataDataMartRawJsonImportID");
        });

        modelBuilder.Entity<StateProvince>(entity =>
        {
            entity.HasKey(e => e.StateProvinceID).HasName("PK_StateProvince_StateProvinceID");

            entity.Property(e => e.StateProvinceID).ValueGeneratedNever();

            entity.HasOne(d => d.Country).WithMany(p => p.StateProvinces).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SupportRequestLog>(entity =>
        {
            entity.HasKey(e => e.SupportRequestLogID).HasName("PK_SupportRequestLog_SupportRequestLogID");
        });

        modelBuilder.Entity<SystemAttribute>(entity =>
        {
            entity.HasKey(e => e.SystemAttributeID).HasName("PK_SystemAttribute_SystemAttributeID");

            entity.HasOne(d => d.AssociatePerfomanceMeasureTaxonomyLevel).WithMany(p => p.SystemAttributeAssociatePerfomanceMeasureTaxonomyLevels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemAttribute_TaxonomyLevel_AssociatePerfomanceMeasureTaxonomyLevelID_TaxonomyLevelID");

            entity.HasOne(d => d.BannerLogoFileResource).WithMany(p => p.SystemAttributeBannerLogoFileResources).HasConstraintName("FK_SystemAttribute_FileResource_BannerLogoFileResourceID_FileResourceID");

            entity.HasOne(d => d.PrimaryContactPerson).WithMany(p => p.SystemAttributes).HasConstraintName("FK_SystemAttribute_Person_PrimaryContactPersonID_PersonID");

            entity.HasOne(d => d.SquareLogoFileResource).WithMany(p => p.SystemAttributeSquareLogoFileResources).HasConstraintName("FK_SystemAttribute_FileResource_SquareLogoFileResourceID_FileResourceID");

            entity.HasOne(d => d.TaxonomyLevel).WithMany(p => p.SystemAttributeTaxonomyLevels).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TabularDataImport>(entity =>
        {
            entity.HasKey(e => e.TabularDataImportID).HasName("PK_TabularDataImport_TabularDataImportID");

            entity.HasOne(d => d.LastProcessedPerson).WithMany(p => p.TabularDataImportLastProcessedPeople).HasConstraintName("FK_TabularDataImport_Person_LastProcessedPersonID_PersonID");

            entity.HasOne(d => d.UploadPerson).WithMany(p => p.TabularDataImportUploadPeople).HasConstraintName("FK_TabularDataImport_Person_UploadPersonID_PersonID");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagID).HasName("PK_Tag_TagID");
        });

        modelBuilder.Entity<TaxonomyBranch>(entity =>
        {
            entity.HasKey(e => e.TaxonomyBranchID).HasName("PK_TaxonomyBranch_TaxonomyBranchID");

            entity.HasOne(d => d.TaxonomyTrunk).WithMany(p => p.TaxonomyBranches).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxonomyLevel>(entity =>
        {
            entity.HasKey(e => e.TaxonomyLevelID).HasName("PK_TaxonomyLevel_TaxonomyLevelID");
        });

        modelBuilder.Entity<TaxonomyTrunk>(entity =>
        {
            entity.HasKey(e => e.TaxonomyTrunkID).HasName("PK_TaxonomyTrunk_TaxonomyTrunkID");
        });

        modelBuilder.Entity<TrainingVideo>(entity =>
        {
            entity.HasKey(e => e.TrainingVideoID).HasName("PK_TrainingVideo_TrainingVideoID");
        });

        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.HasKey(e => e.TreatmentID).HasName("PK_Treatment_TreatmentID");

            entity.HasOne(d => d.Project).WithMany(p => p.Treatments).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TreatmentArea>(entity =>
        {
            entity.HasKey(e => e.TreatmentAreaID).HasName("PK_TreatmentArea_TreatmentAreaID");
        });

        modelBuilder.Entity<TreatmentUpdate>(entity =>
        {
            entity.HasKey(e => e.TreatmentUpdateID).HasName("PK_TreatmentUpdate_TreatmentUpdateID");

            entity.HasOne(d => d.ProjectUpdateBatch).WithMany(p => p.TreatmentUpdates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.VendorID).HasName("PK_Vendor_VendorID");
        });

        modelBuilder.Entity<WashingtonLegislativeDistrict>(entity =>
        {
            entity.HasKey(e => e.WashingtonLegislativeDistrictID).HasName("PK_WashingtonLegislativeDistrict_WashingtonLegislativeDistrictID");
        });

        modelBuilder.Entity<vArcOnlineRawJsonImportIndex>(entity =>
        {
            entity.ToView("vArcOnlineRawJsonImportIndex");
        });

        modelBuilder.Entity<vGeoServerCounty>(entity =>
        {
            entity.ToView("vGeoServerCounty");
        });

        modelBuilder.Entity<vGeoServerPriorityLandscape>(entity =>
        {
            entity.ToView("vGeoServerPriorityLandscape");
        });

        modelBuilder.Entity<vLoaStageFundSourceAllocation>(entity =>
        {
            entity.ToView("vLoaStageFundSourceAllocation");
        });

        modelBuilder.Entity<vLoaStageFundSourceAllocationByProgramIndexProjectCode>(entity =>
        {
            entity.ToView("vLoaStageFundSourceAllocationByProgramIndexProjectCode");
        });

        modelBuilder.Entity<vLoaStageProjectFundSourceAllocation>(entity =>
        {
            entity.ToView("vLoaStageProjectFundSourceAllocation");
        });

        modelBuilder.Entity<vSingularFundSourceAllocation>(entity =>
        {
            entity.ToView("vSingularFundSourceAllocation");
        });

        modelBuilder.Entity<vSocrataDataMartRawJsonImportIndex>(entity =>
        {
            entity.ToView("vSocrataDataMartRawJsonImportIndex");
        });

        modelBuilder.Entity<vTotalTreatedAcresByProject>(entity =>
        {
            entity.ToView("vTotalTreatedAcresByProject");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
