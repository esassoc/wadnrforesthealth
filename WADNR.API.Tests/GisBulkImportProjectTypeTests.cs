using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Tests;

/// <summary>
/// Covers WADNR-2287: the GDB bulk import mis-mapped Project Type, and four other behaviours the
/// MVC-to-API rewrite silently dropped while leaving their configuration editable in the Program
/// admin UI.
///
/// 1. New projects fell back to <c>ProjectTypes.First()</c> — the lowest ProjectTypeID, which in
///    production is "Research and Monitoring" — instead of "Other".
/// 2. <c>AdjustProjectTypeBasedOnTreatmentTypes</c> was never honoured.
/// 3. <c>DataDeriveProjectStage</c> + the ProjectStage crosswalk were never applied.
/// 4. The LeadImplementer organization crosswalk was never applied.
/// 5. <c>ImportAsDetailedLocationInsteadOfTreatments</c> did not gate the treatment proc.
/// 6. <c>GisUploadProgramMergeGrouping</c> did not widen cross-program project matching.
///
/// Note on the treatment proc: the in-memory provider cannot run
/// dbo.procImportTreatmentsFromGisUploadAttempt, so any import that reaches it records a warning and
/// skips the project-type derivation (by design — we never derive from a partial treatment set).
/// Tests that exercise the derivation therefore set ImportAsDetailedLocationInsteadOfTreatments,
/// which is also how they assert behaviour 5.
/// </summary>
[TestClass]
public class GisBulkImportProjectTypeTests
{
    private const int ProgramID = 1;
    private const int SiblingProgramID = 2;
    private const int SourceOrgID = 10;
    private const int SiblingSourceOrgID = 11;
    private const int AttemptID = 100;
    private const int MergeGroupingID = 7;

    private const int IdentifierAttrID = 1;
    private const int NameAttrID = 2;
    private const int StageAttrID = 3;
    private const int LeadImplementerAttrID = 4;
    private const int CompletionDateAttrID = 5;

    // Deliberately the lowest ProjectTypeID in every seed, so a reintroduced `.First()` fallback
    // lands here and fails the test. This mirrors production, where "Research and Monitoring" is
    // ProjectTypeID 2218 — the lowest in dbo.ProjectType.
    private const int ResearchAndMonitoringProjectTypeID = 1;
    private const int CommercialProjectTypeID = 50;
    private const int NonCommercialProjectTypeID = 51;
    private const int PrescribedFireProjectTypeID = 52;
    private const int OtherProjectTypeID = 99;

    private const int DefaultOrganizationID = 1;
    private const int CrosswalkedOrganizationID = 7;
    private const int RelationshipTypeID = 1;

    private const int ExistingProjectID = 500;

    private static WADNRDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new WADNRDbContext(options);
    }

    private static Geometry MakeSquare()
    {
        var factory = new GeometryFactory(new PrecisionModel(), 4326);
        var ring = factory.CreateLinearRing(new[]
        {
            new Coordinate(-120.0, 47.0),
            new Coordinate(-120.0, 47.1),
            new Coordinate(-119.9, 47.1),
            new Coordinate(-119.9, 47.0),
            new Coordinate(-120.0, 47.0),
        });
        return factory.CreatePolygon(ring);
    }

    private static void SeedProjectTypes(WADNRDbContext db, bool includeOther = true)
    {
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = ResearchAndMonitoringProjectTypeID, ProjectTypeName = "Research and Monitoring" });
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = CommercialProjectTypeID, ProjectTypeName = "Commercial vegetation treatment" });
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = NonCommercialProjectTypeID, ProjectTypeName = "Non-commercial vegetation treatment" });
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = PrescribedFireProjectTypeID, ProjectTypeName = "Prescribed fire treatment" });
        if (includeOther)
        {
            db.ProjectTypes.Add(new ProjectType { ProjectTypeID = OtherProjectTypeID, ProjectTypeName = "Other" });
        }
    }

    private sealed class SeedOptions
    {
        public string? ProjectTypeDefaultName { get; init; }
        public bool AdjustProjectTypeBasedOnTreatmentTypes { get; init; }
        public bool DataDeriveProjectStage { get; init; }
        public bool ImportAsDetailedLocationInsteadOfTreatments { get; init; }
        public bool? ImportIsFlattened { get; init; }
        public bool IncludeOtherProjectType { get; init; } = true;
        public int? MergeGroupingID { get; init; }

        public string? StageValue { get; init; }
        public string? LeadImplementerValue { get; init; }
        public string? CompletionDateValue { get; init; }

        public List<(string SourceValue, string MappedValue)> ProjectStageCrossWalks { get; init; } = new();
        public List<(string SourceValue, string MappedValue)> LeadImplementerCrossWalks { get; init; } = new();
    }

    /// <summary>
    /// Seeds a single-feature upload for identifier "PROJ-1" against a source org configured by
    /// <paramref name="options"/>.
    /// </summary>
    private static async Task SeedAsync(WADNRDbContext db, SeedOptions options)
    {
        SeedProjectTypes(db, options.IncludeOtherProjectType);

        db.Organizations.Add(new Organization { OrganizationID = DefaultOrganizationID, OrganizationName = "Default Org", OrganizationShortName = "DEF", IsActive = true });
        db.Organizations.Add(new Organization { OrganizationID = CrosswalkedOrganizationID, OrganizationName = "Crosswalked Org", OrganizationShortName = "XWALK", IsActive = true });

        if (options.MergeGroupingID.HasValue)
        {
            db.GisUploadProgramMergeGroupings.Add(new GisUploadProgramMergeGrouping
            {
                GisUploadProgramMergeGroupingID = options.MergeGroupingID.Value,
                GisUploadProgramMergeGroupingName = "Test Merge Grouping"
            });
        }

        db.GisUploadSourceOrganizations.Add(new GisUploadSourceOrganization
        {
            GisUploadSourceOrganizationID = SourceOrgID,
            GisUploadSourceOrganizationName = "Test Source",
            ProgramID = ProgramID,
            ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
            ProjectTypeDefaultName = options.ProjectTypeDefaultName,
            AdjustProjectTypeBasedOnTreatmentTypes = options.AdjustProjectTypeBasedOnTreatmentTypes,
            DataDeriveProjectStage = options.DataDeriveProjectStage,
            ImportAsDetailedLocationInsteadOfTreatments = options.ImportAsDetailedLocationInsteadOfTreatments,
            ImportIsFlattened = options.ImportIsFlattened,
            GisUploadProgramMergeGroupingID = options.MergeGroupingID,
            DefaultLeadImplementerOrganizationID = DefaultOrganizationID,
            RelationshipTypeForDefaultOrganizationID = RelationshipTypeID,
            ApplyStartDateToProject = false,
            ApplyCompletedDateToProject = false,
        });

        if (options.MergeGroupingID.HasValue)
        {
            // A sibling source org in the same merge grouping, covering a different program.
            db.GisUploadSourceOrganizations.Add(new GisUploadSourceOrganization
            {
                GisUploadSourceOrganizationID = SiblingSourceOrgID,
                GisUploadSourceOrganizationName = "Sibling Source",
                ProgramID = SiblingProgramID,
                ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
                GisUploadProgramMergeGroupingID = options.MergeGroupingID,
                DefaultLeadImplementerOrganizationID = DefaultOrganizationID,
                RelationshipTypeForDefaultOrganizationID = RelationshipTypeID,
            });
        }

        var crossWalkID = 1;
        foreach (var (sourceValue, mappedValue) in options.ProjectStageCrossWalks)
        {
            db.GisCrossWalkDefaults.Add(new GisCrossWalkDefault
            {
                GisCrossWalkDefaultID = crossWalkID++,
                GisUploadSourceOrganizationID = SourceOrgID,
                FieldDefinitionID = FieldDefinition.ProjectStage.FieldDefinitionID,
                GisCrossWalkSourceValue = sourceValue,
                GisCrossWalkMappedValue = mappedValue,
            });
        }

        foreach (var (sourceValue, mappedValue) in options.LeadImplementerCrossWalks)
        {
            db.GisCrossWalkDefaults.Add(new GisCrossWalkDefault
            {
                GisCrossWalkDefaultID = crossWalkID++,
                GisUploadSourceOrganizationID = SourceOrgID,
                FieldDefinitionID = FieldDefinition.LeadImplementerOrganization.FieldDefinitionID,
                GisCrossWalkSourceValue = sourceValue,
                GisCrossWalkMappedValue = mappedValue,
            });
        }

        db.GisUploadAttempts.Add(new GisUploadAttempt
        {
            GisUploadAttemptID = AttemptID,
            GisUploadSourceOrganizationID = SourceOrgID,
            GisUploadAttemptCreatePersonID = 1,
            GisUploadAttemptCreateDate = new DateTime(2026, 8, 17, 0, 0, 0, DateTimeKind.Utc),
        });

        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = IdentifierAttrID, GisMetadataAttributeName = "project_id" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = NameAttrID, GisMetadataAttributeName = "project_name" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = StageAttrID, GisMetadataAttributeName = "status" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = LeadImplementerAttrID, GisMetadataAttributeName = "implementer" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = CompletionDateAttrID, GisMetadataAttributeName = "completed" });

        db.GisFeatures.Add(new GisFeature
        {
            GisFeatureID = 1000,
            GisUploadAttemptID = AttemptID,
            GisFeatureGeometry = MakeSquare(),
            GisImportFeatureKey = 0,
            IsValid = true,
        });

        var metaID = 1;
        void AddMetadata(int attributeID, string? value)
        {
            if (value == null) return;
            db.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute
            {
                GisFeatureMetadataAttributeID = metaID++,
                GisFeatureID = 1000,
                GisMetadataAttributeID = attributeID,
                GisFeatureMetadataAttributeValue = value,
            });
        }

        AddMetadata(IdentifierAttrID, "PROJ-1");
        AddMetadata(NameAttrID, "Test Project");
        AddMetadata(StageAttrID, options.StageValue);
        AddMetadata(LeadImplementerAttrID, options.LeadImplementerValue);
        AddMetadata(CompletionDateAttrID, options.CompletionDateValue);

        await db.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Adds a project already linked to <paramref name="programID"/> that the import will match on
    /// its GIS identifier, so the update path (and the project-type derivation, which keys off
    /// LastUpdateGisUploadAttemptID) can be exercised against pre-seeded treatments.
    /// </summary>
    private static async Task SeedExistingProjectAsync(
        WADNRDbContext db, int programID, int projectTypeID, params (int TreatmentTypeID, decimal? TreatedAcres)[] treatments)
    {
        db.Projects.Add(new Project
        {
            ProjectID = ExistingProjectID,
            ProjectName = "Existing Project",
            FhtProjectNumber = "FHT-2026-00001",
            ProjectGisIdentifier = "PROJ-1",
            ProjectTypeID = projectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Implementation,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
        });

        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = ExistingProjectID, ProgramID = programID });

        var treatmentID = 1;
        foreach (var (treatmentTypeID, treatedAcres) in treatments)
        {
            db.Treatments.Add(new Treatment
            {
                TreatmentID = treatmentID++,
                ProjectID = ExistingProjectID,
                TreatmentTypeID = treatmentTypeID,
                TreatmentDetailedActivityTypeID = TreatmentDetailedActivityType.Other.TreatmentDetailedActivityTypeID,
                TreatmentFootprintAcres = 10m,
                TreatmentTreatedAcres = treatedAcres,
                // Null ProjectLocationID keeps these out of the import's location/treatment cleanup.
                ProjectLocationID = null,
            });
        }

        await db.SaveChangesWithNoAuditingAsync();
    }

    private static GisBulkImportRequest BuildRequest() => new()
    {
        ProjectIdentifierMetadataAttributeID = IdentifierAttrID,
        ProjectNameMetadataAttributeID = NameAttrID,
        ProjectStageMetadataAttributeID = StageAttrID,
        LeadImplementerMetadataAttributeID = LeadImplementerAttrID,
        CompletionDateMetadataAttributeID = CompletionDateAttrID,
    };

    // ---------------------------------------------------------------------------------------
    // 1. Project type fallback
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_FallsBackToOtherProjectType_WhenSourceOrgHasNoDefaultName()
    {
        // The direct WADNR-2287 regression test. "Research and Monitoring" is seeded at the lowest
        // ProjectTypeID, so the old `ProjectTypes.First()` fallback would pick it.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions { ProjectTypeDefaultName = null });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        var project = await db.Projects.SingleAsync();
        Assert.AreEqual(OtherProjectTypeID, project.ProjectTypeID,
            "A source org with no ProjectTypeDefaultName must fall back to \"Other\", not the lowest-ID project type.");
    }

    [TestMethod]
    public async Task ImportProjects_FallsBackToOtherProjectType_WhenDefaultNameMatchesNothing()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions { ProjectTypeDefaultName = "No Such Project Type" });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(OtherProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_UsesConfiguredProjectType_WhenDefaultNameMatchesCaseInsensitively()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions { ProjectTypeDefaultName = "  commercial VEGETATION treatment " });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(CommercialProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_Throws_WhenNoDefaultNameAndNoOtherProjectType()
    {
        // Better to fail the import than to silently publish a wrong project type.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions { ProjectTypeDefaultName = null, IncludeOtherProjectType = false });

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest()));
    }

    // ---------------------------------------------------------------------------------------
    // 2. AdjustProjectTypeBasedOnTreatmentTypes
    // ---------------------------------------------------------------------------------------

    private static SeedOptions AdjustTypeOptions(bool adjust = true, bool? flattened = null) => new()
    {
        ProjectTypeDefaultName = null,
        AdjustProjectTypeBasedOnTreatmentTypes = adjust,
        // Skips the treatment proc, which the in-memory provider cannot run.
        ImportAsDetailedLocationInsteadOfTreatments = true,
        ImportIsFlattened = flattened,
    };

    [TestMethod]
    public async Task ImportProjects_DerivesProjectTypeFromSoleTreatmentType_WhenAdjustFlagSet()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, AdjustTypeOptions());
        await SeedExistingProjectAsync(db, ProgramID, ResearchAndMonitoringProjectTypeID,
            (TreatmentType.NonCommercial.TreatmentTypeID, 5m),
            (TreatmentType.NonCommercial.TreatmentTypeID, 3m));

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsUpdated);
        Assert.AreEqual(NonCommercialProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_DerivesPrescribedFireProjectType_WhenSoleTreatmentTypeIsPrescribedFire()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, AdjustTypeOptions());
        await SeedExistingProjectAsync(db, ProgramID, ResearchAndMonitoringProjectTypeID,
            (TreatmentType.PrescribedFire.TreatmentTypeID, 5m));

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(PrescribedFireProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_LeavesProjectTypeAlone_WhenTreatmentTypesAreMixed()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, AdjustTypeOptions());
        await SeedExistingProjectAsync(db, ProgramID, ResearchAndMonitoringProjectTypeID,
            (TreatmentType.NonCommercial.TreatmentTypeID, 5m),
            (TreatmentType.PrescribedFire.TreatmentTypeID, 5m));

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(ResearchAndMonitoringProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID,
            "A mixed treatment set is ambiguous, so the project type must be left alone.");
    }

    [TestMethod]
    public async Task ImportProjects_LeavesProjectTypeAlone_WhenProjectHasNoTreatments()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, AdjustTypeOptions());
        await SeedExistingProjectAsync(db, ProgramID, ResearchAndMonitoringProjectTypeID);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(ResearchAndMonitoringProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_LeavesProjectTypeAlone_WhenAdjustFlagNotSet()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, AdjustTypeOptions(adjust: false));
        await SeedExistingProjectAsync(db, ProgramID, ResearchAndMonitoringProjectTypeID,
            (TreatmentType.NonCommercial.TreatmentTypeID, 5m));

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(ResearchAndMonitoringProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_IgnoresZeroAcreTreatments_WhenImportIsFlattened()
    {
        // A flattened source writes one row per acreage column, so only the rows with treated acres
        // describe what actually happened. Without the filter this project reads as "mixed".
        await using var db = NewInMemoryContext();
        await SeedAsync(db, AdjustTypeOptions(flattened: true));
        await SeedExistingProjectAsync(db, ProgramID, ResearchAndMonitoringProjectTypeID,
            (TreatmentType.Commercial.TreatmentTypeID, 12m),
            (TreatmentType.PrescribedFire.TreatmentTypeID, 0m),
            (TreatmentType.NonCommercial.TreatmentTypeID, null));

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(CommercialProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_CountsZeroAcreTreatments_WhenImportIsNotFlattened()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, AdjustTypeOptions(flattened: false));
        await SeedExistingProjectAsync(db, ProgramID, ResearchAndMonitoringProjectTypeID,
            (TreatmentType.Commercial.TreatmentTypeID, 12m),
            (TreatmentType.PrescribedFire.TreatmentTypeID, 0m));

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(ResearchAndMonitoringProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID,
            "Without the flattened flag every treatment counts, so this set is mixed and must be left alone.");
    }

    // ---------------------------------------------------------------------------------------
    // 3. DataDeriveProjectStage + ProjectStage crosswalk
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_UsesConfiguredDefaultStage_WhenDataDeriveProjectStageNotSet()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            DataDeriveProjectStage = false,
            StageValue = "COMPLETE",
            ProjectStageCrossWalks = { ("COMPLETE", "Completed") },
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual((int)ProjectStageEnum.Implementation, (await db.Projects.SingleAsync()).ProjectStageID);
    }

    [TestMethod]
    public async Task ImportProjects_UsesCrosswalkedStage_WhenDataDeriveProjectStageSet()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            DataDeriveProjectStage = true,
            StageValue = "complete",
            CompletionDateValue = "2026-05-01",
            ProjectStageCrossWalks = { ("COMPLETE", "Completed") },
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(ProjectStage.Completed.ProjectStageID, (await db.Projects.SingleAsync()).ProjectStageID);
    }

    [TestMethod]
    public async Task ImportProjects_FallsBackInsteadOfThrowing_WhenStageValueHasNoCrosswalkRow()
    {
        // Legacy used Single(...) here and blew up the whole import on an unmapped source value.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            DataDeriveProjectStage = true,
            StageValue = "SOMETHING UNMAPPED",
            CompletionDateValue = "2026-05-01",
            ProjectStageCrossWalks = { ("COMPLETE", "Completed") },
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        Assert.AreEqual((int)ProjectStageEnum.Implementation, (await db.Projects.SingleAsync()).ProjectStageID,
            "An unmapped stage value must fall back to the configured default rather than throw.");
    }

    [TestMethod]
    public async Task ImportProjects_UsesPlannedStage_WhenDerivingWithNoCompletionDate()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            DataDeriveProjectStage = true,
            StageValue = null,
            CompletionDateValue = null,
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual((int)ProjectStageEnum.Planned, (await db.Projects.SingleAsync()).ProjectStageID);
    }

    // ---------------------------------------------------------------------------------------
    // 4. LeadImplementer organization crosswalk
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_UsesCrosswalkedLeadImplementer_WhenMapped()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            LeadImplementerValue = "usfs",
            LeadImplementerCrossWalks = { ("USFS", "Crosswalked Org") },
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var projectOrganization = await db.ProjectOrganizations.SingleAsync();
        Assert.AreEqual(CrosswalkedOrganizationID, projectOrganization.OrganizationID);
        Assert.AreEqual(RelationshipTypeID, projectOrganization.RelationshipTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_UsesDefaultLeadImplementer_WhenValueUnmapped()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            LeadImplementerValue = "Some Other Agency",
            LeadImplementerCrossWalks = { ("USFS", "Crosswalked Org") },
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(DefaultOrganizationID, (await db.ProjectOrganizations.SingleAsync()).OrganizationID);
    }

    [TestMethod]
    public async Task ImportProjects_UsesDefaultLeadImplementer_WhenMappedOrganizationDoesNotExist()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            LeadImplementerValue = "USFS",
            LeadImplementerCrossWalks = { ("USFS", "An Organization That Was Deleted") },
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(DefaultOrganizationID, (await db.ProjectOrganizations.SingleAsync()).OrganizationID);
    }

    [TestMethod]
    public async Task ImportProjects_UsesDefaultLeadImplementer_WhenNoCrosswalkConfigured()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions { LeadImplementerValue = "USFS" });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(DefaultOrganizationID, (await db.ProjectOrganizations.SingleAsync()).OrganizationID);
    }

    // ---------------------------------------------------------------------------------------
    // 5. ImportAsDetailedLocationInsteadOfTreatments gates the treatment proc
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_SkipsTreatmentProc_WhenImportAsDetailedLocationInsteadOfTreatments()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions { ImportAsDetailedLocationInsteadOfTreatments = true });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        Assert.AreEqual(0, result.Warnings.Count,
            "The treatment proc must not be called at all when the source imports detailed locations instead.");
    }

    [TestMethod]
    public async Task ImportProjects_CallsTreatmentProc_WhenImportAsDetailedLocationInsteadOfTreatmentsNotSet()
    {
        // The in-memory provider cannot run the proc, so reaching it surfaces as a warning — which
        // is exactly the signal that the gate above is doing something.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions { ImportAsDetailedLocationInsteadOfTreatments = false });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        Assert.AreEqual(1, result.Warnings.Count);
    }

    [TestMethod]
    public async Task ImportProjects_SkipsProjectTypeDerivation_WhenTreatmentImportFails()
    {
        // Never derive a project type from a partially imported treatment set.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            AdjustProjectTypeBasedOnTreatmentTypes = true,
            ImportAsDetailedLocationInsteadOfTreatments = false,
        });
        await SeedExistingProjectAsync(db, ProgramID, ResearchAndMonitoringProjectTypeID,
            (TreatmentType.NonCommercial.TreatmentTypeID, 5m));

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.Warnings.Count, "Expected the in-memory treatment proc call to fail.");
        Assert.AreEqual(ResearchAndMonitoringProjectTypeID, (await db.Projects.SingleAsync()).ProjectTypeID);
    }

    // ---------------------------------------------------------------------------------------
    // 6. GisUploadProgramMergeGrouping widens cross-program matching
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_MatchesProjectInSiblingProgram_WhenSourceOrgIsInAMergeGrouping()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            MergeGroupingID = MergeGroupingID,
            ImportAsDetailedLocationInsteadOfTreatments = true,
        });
        // The project lives under the sibling program, not the importing source org's program.
        await SeedExistingProjectAsync(db, SiblingProgramID, OtherProjectTypeID);

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(0, result.ProjectsCreated, "The sibling program's project must be matched, not duplicated.");
        Assert.AreEqual(1, result.ProjectsUpdated);
        Assert.AreEqual(1, await db.Projects.CountAsync());

        // The importing source org's program is still linked to the matched project.
        Assert.IsTrue(await db.ProjectPrograms.AnyAsync(x => x.ProjectID == ExistingProjectID && x.ProgramID == ProgramID));
        Assert.IsTrue(await db.ProjectPrograms.AnyAsync(x => x.ProjectID == ExistingProjectID && x.ProgramID == SiblingProgramID));
    }

    [TestMethod]
    public async Task ImportProjects_DoesNotMatchProjectInOtherProgram_WhenSourceOrgHasNoMergeGrouping()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions { ImportAsDetailedLocationInsteadOfTreatments = true });
        await SeedExistingProjectAsync(db, SiblingProgramID, OtherProjectTypeID);

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        Assert.AreEqual(0, result.ProjectsUpdated);
        Assert.AreEqual(2, await db.Projects.CountAsync());
    }

    [TestMethod]
    public async Task ImportProjects_CreatesLocationUnderImportingProgram_WhenMatchedViaMergeGrouping()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            MergeGroupingID = MergeGroupingID,
            ImportAsDetailedLocationInsteadOfTreatments = true,
        });
        await SeedExistingProjectAsync(db, SiblingProgramID, OtherProjectTypeID);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var location = await db.ProjectLocations.SingleAsync();
        Assert.AreEqual(ExistingProjectID, location.ProjectID);
        Assert.AreEqual(ProgramID, location.ProgramID,
            "Locations belong to the importing source org's program even when the project was matched across the grouping.");
    }
}
