using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Tests;

/// <summary>
/// Covers the legacy GIS import behaviours the MVC-to-API rewrite dropped, found by auditing
/// GisProjectBulkUpdateController against GisBulkImports.ImportProjectsAsync. Each of these was
/// still configurable in the Program admin UI, so it read as working while doing nothing:
///
/// 1. GisExcludeIncludeColumns whitelist / blacklist filtering of features.
/// 2. Private landowner ProjectPerson creation from the Landowner column.
/// 3. DNR Service Forestry Regional Coordinator assignment on new LOA projects.
/// 4. GisUploadSourceOrganization.RequiresCompletionDate() skip.
/// 5. Setting Project.ProjectLocationPoint when the treatment proc is skipped.
/// 6. Clearing the attempt's staged GIS features after a successful import.
/// 7. Block-list entries that point at a ProjectID rather than an identifier or name.
///
/// The source org runs against the Landowner Assistance program, because that is the program whose
/// production configuration exercises most of this (landowner column, regional coordinators).
/// </summary>
[TestClass]
public class GisBulkImportLegacyParityTests
{
    // Fully qualified: bare `Program` binds to the API's ASP.NET entry-point class in this assembly.
    private static readonly int ProgramID = WADNR.EFModels.Entities.Program.LandownerAssistanceProgramID;
    private const int OtherProgramID = 99;
    private const int SourceOrgID = 10;
    private const int AttemptID = 100;

    private const int IdentifierAttrID = 1;
    private const int NameAttrID = 2;
    private const int LandownerAttrID = 3;
    private const int CompletionDateAttrID = 4;
    private const int TechniqueAttrID = 5;

    private const int OtherProjectTypeID = 99;
    private const int OrganizationID = 1;
    private const int RelationshipTypeID = 1;
    private const int RegionCoordinatorPersonID = 555;
    private const int DNRUplandRegionID = 1;

    private static WADNRDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new WADNRDbContext(options);
    }

    private static Geometry MakeSquare(double offset = 0)
    {
        var factory = new GeometryFactory(new PrecisionModel(), 4326);
        return factory.CreatePolygon(factory.CreateLinearRing(new[]
        {
            new Coordinate(-120.0 + offset, 47.0),
            new Coordinate(-120.0 + offset, 47.1),
            new Coordinate(-119.9 + offset, 47.1),
            new Coordinate(-119.9 + offset, 47.0),
            new Coordinate(-120.0 + offset, 47.0),
        }));
    }

    /// <summary>One GIS feature's worth of metadata.</summary>
    private sealed record FeatureSpec(
        string Identifier,
        string Name = "Test Project",
        string? Landowner = null,
        string? CompletionDate = null,
        string? Technique = null);

    private sealed class SeedOptions
    {
        public int ProjectStageDefaultID { get; init; } = (int)ProjectStageEnum.Implementation;
        public bool DataDeriveProjectStage { get; init; }
        public bool ImportAsDetailedLocationInsteadOfTreatments { get; init; }
        public List<FeatureSpec> Features { get; init; } = new();

        /// <summary>Configured include/exclude column, if any: (columnName, isWhitelist, values).</summary>
        public (string ColumnName, bool IsWhitelist, string[] Values)? ExcludeIncludeColumn { get; init; }
    }

    private static async Task SeedAsync(WADNRDbContext db, SeedOptions options)
    {
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = OtherProjectTypeID, ProjectTypeName = "Other" });
        db.Organizations.Add(new Organization
        {
            OrganizationID = OrganizationID,
            OrganizationName = "Default Org",
            OrganizationShortName = "DEF",
            IsActive = true
        });

        db.GisUploadSourceOrganizations.Add(new GisUploadSourceOrganization
        {
            GisUploadSourceOrganizationID = SourceOrgID,
            GisUploadSourceOrganizationName = "Parity Test Source",
            ProgramID = ProgramID,
            ProjectStageDefaultID = options.ProjectStageDefaultID,
            ProjectTypeDefaultName = null,
            DataDeriveProjectStage = options.DataDeriveProjectStage,
            ImportAsDetailedLocationInsteadOfTreatments = options.ImportAsDetailedLocationInsteadOfTreatments,
            DefaultLeadImplementerOrganizationID = OrganizationID,
            RelationshipTypeForDefaultOrganizationID = RelationshipTypeID,
            ApplyStartDateToProject = false,
            ApplyCompletedDateToProject = false,
        });

        if (options.ExcludeIncludeColumn.HasValue)
        {
            var (columnName, isWhitelist, values) = options.ExcludeIncludeColumn.Value;
            db.GisExcludeIncludeColumns.Add(new GisExcludeIncludeColumn
            {
                GisExcludeIncludeColumnID = 1,
                GisUploadSourceOrganizationID = SourceOrgID,
                GisDefaultMappingColumnName = columnName,
                IsWhitelist = isWhitelist,
            });
            var valueID = 1;
            foreach (var value in values)
            {
                db.GisExcludeIncludeColumnValues.Add(new GisExcludeIncludeColumnValue
                {
                    GisExcludeIncludeColumnValueID = valueID++,
                    GisExcludeIncludeColumnID = 1,
                    GisExcludeIncludeColumnValueForFiltering = value,
                });
            }
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
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = LandownerAttrID, GisMetadataAttributeName = "landowner" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = CompletionDateAttrID, GisMetadataAttributeName = "completed" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = TechniqueAttrID, GisMetadataAttributeName = "technique_cd" });

        var featureID = 1000;
        var metaID = 1;
        for (var i = 0; i < options.Features.Count; i++)
        {
            var spec = options.Features[i];
            db.GisFeatures.Add(new GisFeature
            {
                GisFeatureID = featureID,
                GisUploadAttemptID = AttemptID,
                GisFeatureGeometry = MakeSquare(i * 0.5),
                GisImportFeatureKey = i,
                IsValid = true,
            });

            void AddMetadata(int attributeID, string? value)
            {
                if (value == null) return;
                db.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute
                {
                    GisFeatureMetadataAttributeID = metaID++,
                    GisFeatureID = featureID,
                    GisMetadataAttributeID = attributeID,
                    GisFeatureMetadataAttributeValue = value,
                });
            }

            AddMetadata(IdentifierAttrID, spec.Identifier);
            AddMetadata(NameAttrID, spec.Name);
            AddMetadata(LandownerAttrID, spec.Landowner);
            AddMetadata(CompletionDateAttrID, spec.CompletionDate);
            AddMetadata(TechniqueAttrID, spec.Technique);

            featureID++;
        }

        await db.SaveChangesWithNoAuditingAsync();
    }

    private static GisBulkImportRequest BuildRequest(bool mapLandowner = false) => new()
    {
        ProjectIdentifierMetadataAttributeID = IdentifierAttrID,
        ProjectNameMetadataAttributeID = NameAttrID,
        CompletionDateMetadataAttributeID = CompletionDateAttrID,
        PrivateLandownerMetadataAttributeID = mapLandowner ? LandownerAttrID : null,
    };

    // ---------------------------------------------------------------------------------------
    // 1. Include / exclude column filtering
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_DropsBlacklistedFeatures()
    {
        // Mirrors DNR State Lands' production configuration: a blacklist on technique_cd.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ExcludeIncludeColumn = ("technique_cd", false, new[] { "NATURAL", "SEED_GRASS" }),
            Features =
            {
                new FeatureSpec("PROJ-1", Technique: "PCT"),
                new FeatureSpec("PROJ-2", Technique: "NATURAL"),
                new FeatureSpec("PROJ-3", Technique: "SEED_GRASS"),
            }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated, "Only the non-blacklisted feature may produce a project.");
        Assert.AreEqual("PROJ-1", (await db.Projects.SingleAsync()).ProjectGisIdentifier);
    }

    [TestMethod]
    public async Task ImportProjects_KeepsOnlyWhitelistedFeatures()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ExcludeIncludeColumn = ("technique_cd", true, new[] { "PCT" }),
            Features =
            {
                new FeatureSpec("PROJ-1", Technique: "PCT"),
                new FeatureSpec("PROJ-2", Technique: "NATURAL"),
            }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        Assert.AreEqual("PROJ-1", (await db.Projects.SingleAsync()).ProjectGisIdentifier);
    }

    [TestMethod]
    public async Task ImportProjects_MatchesFilterValuesCaseInsensitively()
    {
        // Legacy compared ordinally, but the upload path lowercases attribute names, so an ordinal
        // match on a mixed-case configured column name would silently never fire.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ExcludeIncludeColumn = ("TECHNIQUE_CD", false, new[] { "natural" }),
            Features =
            {
                new FeatureSpec("PROJ-1", Technique: "PCT"),
                new FeatureSpec("PROJ-2", Technique: "NATURAL"),
            }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
    }

    [TestMethod]
    public async Task ImportProjects_ReportsHowManyFeaturesWereExcluded()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            ExcludeIncludeColumn = ("technique_cd", false, new[] { "NATURAL" }),
            Features =
            {
                new FeatureSpec("PROJ-1", Technique: "PCT"),
                new FeatureSpec("PROJ-2", Technique: "NATURAL"),
            }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.Warnings.Count, "Excluding features silently would look like data loss.");
        StringAssert.Contains(result.Warnings[0], "Excluded 1 of 2 features");
    }

    [TestMethod]
    public async Task ImportProjects_IgnoresFilterConfiguredAgainstAnAbsentColumn()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ExcludeIncludeColumn = ("no_such_column", true, new[] { "PCT" }),
            Features = { new FeatureSpec("PROJ-1", Technique: "PCT") }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated,
            "A whitelist on a column this upload doesn't carry must not silently discard everything.");
    }

    // ---------------------------------------------------------------------------------------
    // 2. Private landowners
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_CreatesPersonAndProjectPerson_ForNewLandowner()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", Landowner: "Nakamura, Rose") }
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest(mapLandowner: true));

        var person = await db.People.SingleAsync();
        Assert.AreEqual("Rose", person.FirstName, "\"Last, First\" must split on the comma.");
        Assert.AreEqual("Nakamura", person.LastName);
        Assert.IsTrue(person.CreatedAsPartOfBulkImport);

        var projectPerson = await db.ProjectPeople.SingleAsync();
        Assert.AreEqual(person.PersonID, projectPerson.PersonID);
        Assert.AreEqual(ProjectPersonRelationshipType.PrivateLandowner.ProjectPersonRelationshipTypeID,
            projectPerson.ProjectPersonRelationshipTypeID);
        Assert.AreEqual(1, await db.PersonRoles.CountAsync(x => x.PersonID == person.PersonID && x.RoleID == Role.Unassigned.RoleID));
    }

    [TestMethod]
    public async Task ImportProjects_TreatsUnpunctuatedLandownerAsAWholeFirstName()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", Landowner: "Cascade Timber LLC") }
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest(mapLandowner: true));

        var person = await db.People.SingleAsync();
        Assert.AreEqual("Cascade Timber LLC", person.FirstName);
        Assert.IsNull(person.LastName);
    }

    [TestMethod]
    public async Task ImportProjects_ReusesExistingPerson_WhenLandownerAlreadyExists()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", Landowner: "nakamura, rose") }
        });
        db.People.Add(new Person
        {
            PersonID = 42,
            FirstName = "Rose",
            LastName = "Nakamura",
            CreateDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
        });
        await db.SaveChangesWithNoAuditingAsync();

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest(mapLandowner: true));

        Assert.AreEqual(1, await db.People.CountAsync(), "Matching is case-insensitive, so no duplicate Person.");
        Assert.AreEqual(42, (await db.ProjectPeople.SingleAsync()).PersonID);
    }

    [TestMethod]
    public async Task ImportProjects_CreatesLandownerOnce_WhenTwoFeaturesShareTheSameName()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features =
            {
                new FeatureSpec("PROJ-1", Landowner: "Nakamura, Rose"),
                new FeatureSpec("PROJ-2", Landowner: "Nakamura, Rose"),
            }
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest(mapLandowner: true));

        Assert.AreEqual(1, await db.People.CountAsync(),
            "The in-memory index must be kept in step, or each project invents its own copy of the person.");
        Assert.AreEqual(2, await db.ProjectPeople.CountAsync());
    }

    [TestMethod]
    public async Task ImportProjects_SkipsLandownerValueThatYieldsNoName()
    {
        // "Smith," splits to an empty first name. Legacy would have created a nameless Person.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", Landowner: "Smith,") }
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest(mapLandowner: true));

        Assert.AreEqual(0, await db.People.CountAsync(), "A landowner value with no first name must not create a Person.");
        Assert.AreEqual(0, await db.ProjectPeople.CountAsync());
    }

    [TestMethod]
    public async Task ImportProjects_LeavesLandownersAlone_WhenLandownerColumnNotMapped()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", Landowner: "Nakamura, Rose") }
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest(mapLandowner: false));

        Assert.AreEqual(0, await db.People.CountAsync());
        Assert.AreEqual(0, await db.ProjectPeople.CountAsync());
    }

    // ---------------------------------------------------------------------------------------
    // 3. Service Forestry Regional Coordinator
    // ---------------------------------------------------------------------------------------

    private static async Task SeedRegionWithCoordinatorAsync(WADNRDbContext db, int? coordinatorPersonID)
    {
        if (coordinatorPersonID.HasValue)
        {
            db.People.Add(new Person
            {
                PersonID = coordinatorPersonID.Value,
                FirstName = "Regional",
                LastName = "Coordinator",
                CreateDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
            });
        }

        db.DNRUplandRegions.Add(new DNRUplandRegion
        {
            DNRUplandRegionID = DNRUplandRegionID,
            DNRUplandRegionName = "Northeast",
            DNRUplandRegionLocation = MakeSquare(),
            DNRUplandRegionCoordinatorID = coordinatorPersonID,
        });
        await db.SaveChangesWithNoAuditingAsync();
    }

    [TestMethod]
    public async Task ImportProjects_AssignsRegionalCoordinator_ToNewLandownerAssistanceProject()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1") }
        });
        await SeedRegionWithCoordinatorAsync(db, RegionCoordinatorPersonID);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var projectPerson = await db.ProjectPeople.SingleAsync();
        Assert.AreEqual(RegionCoordinatorPersonID, projectPerson.PersonID);
        Assert.AreEqual(ProjectPersonRelationshipType.ServiceForestryRegionalCoordinator.ProjectPersonRelationshipTypeID,
            projectPerson.ProjectPersonRelationshipTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_AssignsNoCoordinator_WhenRegionHasNone()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1") }
        });
        await SeedRegionWithCoordinatorAsync(db, coordinatorPersonID: null);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(0, await db.ProjectPeople.CountAsync());
    }

    [TestMethod]
    public async Task ImportProjects_AssignsCoordinatorOnlyOnce_WhenRunTwice()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1") }
        });
        await SeedRegionWithCoordinatorAsync(db, RegionCoordinatorPersonID);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());
        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, await db.ProjectPeople.CountAsync(),
            "Re-running the import must not stack duplicate coordinator rows.");
    }

    // ---------------------------------------------------------------------------------------
    // 4. RequiresCompletionDate
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_SkipsProject_WhenStageDefaultIsCompletedAndNoCompletionDate()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ProjectStageDefaultID = ProjectStage.Completed.ProjectStageID,
            DataDeriveProjectStage = false,
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features =
            {
                new FeatureSpec("PROJ-1", CompletionDate: "2026-05-01"),
                new FeatureSpec("PROJ-2", CompletionDate: null),
            }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        Assert.AreEqual("PROJ-1", (await db.Projects.SingleAsync()).ProjectGisIdentifier);
        Assert.IsTrue(result.Warnings.Any(w => w.Contains("Skipped 1 project")),
            $"Expected the skip to be reported. Warnings: {string.Join(" | ", result.Warnings)}");
    }

    [TestMethod]
    public async Task ImportProjects_DoesNotSkip_WhenStageIsDerivedFromData()
    {
        // RequiresCompletionDate() is false when the stage comes from the data, so nothing is dropped.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ProjectStageDefaultID = ProjectStage.Completed.ProjectStageID,
            DataDeriveProjectStage = true,
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", CompletionDate: null) }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
    }

    [TestMethod]
    public async Task ImportProjects_DoesNotSkip_WhenStageDefaultIsNotCompleted()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", CompletionDate: null) }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
    }

    // ---------------------------------------------------------------------------------------
    // 5. Simple location point when the treatment proc is skipped
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_SetsProjectLocationPoint_WhenTreatmentProcIsSkipped()
    {
        // The proc's final statement normally sets these; gating it off for detailed-location
        // sources would otherwise leave them with no simple location at all.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1") }
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var project = await db.Projects.SingleAsync();
        Assert.IsNotNull(project.ProjectLocationPoint, "Expected a centroid derived from the imported project areas.");
        Assert.AreEqual((int)ProjectLocationSimpleTypeEnum.PointOnMap, project.ProjectLocationSimpleTypeID);
    }

    [TestMethod]
    public async Task ImportProjects_LeavesLocationPointNull_WhenBlockedProjectHasNoProjectAreas()
    {
        // Everything in the upload is blocked, so no project areas exist to build a centroid from.
        // The centroid pass must handle that rather than throwing on an empty geometry set.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", Name: "Renamed") }
        });
        db.Projects.Add(new Project
        {
            ProjectID = 500,
            ProjectName = "Blocked",
            FhtProjectNumber = "FHT-2026-00001",
            ProjectGisIdentifier = "PROJ-1",
            ProjectTypeID = OtherProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Implementation,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
        });
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = 500, ProgramID = ProgramID });
        db.ProjectImportBlockLists.Add(new ProjectImportBlockList
        {
            ProjectImportBlockListID = 1,
            ProgramID = ProgramID,
            ProjectID = 500,
        });
        await db.SaveChangesWithNoAuditingAsync();

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsBlocked);
        db.ChangeTracker.Clear();
        Assert.IsNull((await db.Projects.SingleAsync()).ProjectLocationPoint);
    }

    // ---------------------------------------------------------------------------------------
    // Update-path branches
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_ApprovesDraftProject_AndFillsStartDateAndDescription_OnUpdate()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", CompletionDate: "2026-06-01") }
        });
        var sourceOrg = db.GisUploadSourceOrganizations.Single();
        sourceOrg.ApplyStartDateToProject = true;
        sourceOrg.ProjectDescriptionDefaultText = "Imported from the source organization.";

        db.Projects.Add(new Project
        {
            ProjectID = 500,
            ProjectName = "Existing",
            FhtProjectNumber = "FHT-2026-00001",
            ProjectGisIdentifier = "PROJ-1",
            ProjectTypeID = OtherProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Planned,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Draft,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
            ProjectDescription = null,
        });
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = 500, ProgramID = ProgramID });
        await db.SaveChangesWithNoAuditingAsync();

        var request = DateRequest();
        request.StartDateMetadataAttributeID = CompletionDateAttrID; // reuse the same column as a start date

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, request);

        Assert.AreEqual(1, result.ProjectsUpdated);
        db.ChangeTracker.Clear();
        var project = await db.Projects.SingleAsync();
        Assert.AreEqual((int)ProjectApprovalStatusEnum.Approved, project.ProjectApprovalStatusID,
            "A Draft project moving to a non-Planned stage must be auto-approved.");
        Assert.AreEqual(new DateOnly(2026, 6, 1), project.PlannedDate);
        Assert.AreEqual("Imported from the source organization.", project.ProjectDescription,
            "An empty description must be filled from the source organization default.");
    }

    [TestMethod]
    public async Task ImportProjects_CreatesAndUpdatesInTheSamePass_WithoutCrossingTheTwo()
    {
        // The import writes in phases over the whole batch rather than a transaction per project, so
        // creates and updates now share preloaded state: one query for the existing projects, one for
        // the FHT number block, one for the other-program treatment dates. Every other test here is
        // all-create or all-update, which leaves that sharing untested — a create drawing an existing
        // project's row, or an update consuming a reserved FHT number, would go unnoticed.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features =
            {
                new FeatureSpec("PROJ-EXISTING", Name: "Renamed By Import"),
                new FeatureSpec("PROJ-NEW", Name: "Brand New"),
            }
        });

        const string existingFhtProjectNumber = "FHT-2026-00001";
        db.Projects.Add(new Project
        {
            ProjectID = 500,
            ProjectName = "Existing",
            FhtProjectNumber = existingFhtProjectNumber,
            ProjectGisIdentifier = "PROJ-EXISTING",
            ProjectTypeID = OtherProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Planned,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
        });
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = 500, ProgramID = ProgramID });
        await db.SaveChangesWithNoAuditingAsync();

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated, "PROJ-NEW has no match and must be created.");
        Assert.AreEqual(1, result.ProjectsUpdated, "PROJ-EXISTING matches project 500 and must be updated, not duplicated.");
        Assert.AreEqual(2, result.LocationsCreated);

        db.ChangeTracker.Clear();
        var projectsByIdentifier = await db.Projects.AsNoTracking()
            .ToDictionaryAsync(p => p.ProjectGisIdentifier!, p => p);
        Assert.AreEqual(2, projectsByIdentifier.Count, "Exactly one project per identifier.");

        var updated = projectsByIdentifier["PROJ-EXISTING"];
        Assert.AreEqual(500, updated.ProjectID);
        Assert.AreEqual("Renamed By Import", updated.ProjectName);
        Assert.AreEqual(existingFhtProjectNumber, updated.FhtProjectNumber,
            "An update must never renumber a project — the allocator is for the create path only.");

        var created = projectsByIdentifier["PROJ-NEW"];
        Assert.AreEqual("Brand New", created.ProjectName);
        Assert.AreNotEqual(existingFhtProjectNumber, created.FhtProjectNumber,
            "The reserved block must start past the highest number already in use, or this collides "
            + "with AK_Project_FhtProjectNumber on a real database.");
        StringAssert.StartsWith(created.FhtProjectNumber, $"FHT-{DateTime.Now.Year}-");

        // The created project must carry the create-path stamps and the updated one must not gain them.
        Assert.AreEqual(AttemptID, created.CreateGisUploadAttemptID);
        Assert.IsNull(updated.CreateGisUploadAttemptID,
            "A project the import merely updated must not be restamped as created by this attempt.");
        Assert.AreEqual(AttemptID, updated.LastUpdateGisUploadAttemptID);

        // The lead implementer relationship is create-path only, so exactly one must exist.
        Assert.AreEqual(1, await db.ProjectOrganizations.CountAsync(),
            "Only the created project gets a lead implementer row.");
        Assert.AreEqual(created.ProjectID, (await db.ProjectOrganizations.AsNoTracking().SingleAsync()).ProjectID);
    }

    // ---------------------------------------------------------------------------------------
    // 6. Staged feature cleanup
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_ClearsStagedFeatures_AfterSuccessfulImport()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1") }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(0, result.Warnings.Count);
        Assert.AreEqual(0, await db.GisFeatures.CountAsync());
        Assert.AreEqual(0, await db.GisFeatureMetadataAttributes.CountAsync());
    }

    [TestMethod]
    public async Task ImportProjects_RetainsStagedFeatures_WhenAnythingWentWrong()
    {
        // The treatment proc can't run on the in-memory provider, which produces a warning — exactly
        // the case where the staged data must survive so the failure can be diagnosed.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = false,
            Features = { new FeatureSpec("PROJ-1") }
        });

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreNotEqual(0, result.Warnings.Count);
        Assert.AreEqual(1, await db.GisFeatures.CountAsync());
    }

    // ---------------------------------------------------------------------------------------
    // 8. Date resolution (epoch fallback, earliest/latest across features, cross-program widening)
    // ---------------------------------------------------------------------------------------

    private static SeedOptions DateOptions(params FeatureSpec[] features) => new()
    {
        ImportAsDetailedLocationInsteadOfTreatments = true,
        Features = features.ToList(),
    };

    private static GisBulkImportRequest DateRequest() => new()
    {
        ProjectIdentifierMetadataAttributeID = IdentifierAttrID,
        ProjectNameMetadataAttributeID = NameAttrID,
        CompletionDateMetadataAttributeID = CompletionDateAttrID,
    };

    [TestMethod]
    public async Task ImportProjects_ParsesEpochMillisecondDates_FromArcGisOnline()
    {
        // The nightly LOA / USFS jobs stage Esri date fields as epoch milliseconds. Parsing them only
        // with DateTime.TryParse meant those imports silently resolved no dates at all.
        var epochMillis = new DateTimeOffset(new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeMilliseconds();

        await using var db = NewInMemoryContext();
        await SeedAsync(db, DateOptions(new FeatureSpec("PROJ-1", CompletionDate: epochMillis.ToString())));
        db.GisUploadSourceOrganizations.Single().ApplyCompletedDateToProject = true;
        await db.SaveChangesWithNoAuditingAsync();

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, DateRequest());

        Assert.AreEqual(new DateOnly(2026, 5, 1), (await db.Projects.SingleAsync()).CompletionDate);
    }

    [TestMethod]
    public async Task ImportProjects_TakesLatestCompletionDateAcrossAllFeaturesOfAProject()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, DateOptions(
            new FeatureSpec("PROJ-1", CompletionDate: "2026-01-15"),
            new FeatureSpec("PROJ-1", CompletionDate: "2026-07-20"),
            new FeatureSpec("PROJ-1", CompletionDate: "2026-03-02")));
        db.GisUploadSourceOrganizations.Single().ApplyCompletedDateToProject = true;
        await db.SaveChangesWithNoAuditingAsync();

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, DateRequest());

        Assert.AreEqual(new DateOnly(2026, 7, 20), (await db.Projects.SingleAsync()).CompletionDate,
            "The completion date must be the latest across the project's features, not the first one seen.");
    }

    [TestMethod]
    public async Task ImportProjects_PrefersRealDatesOverEpochInterpretation()
    {
        // The epoch fallback only applies when nothing parsed as a date, so a normal GDB upload is
        // never reinterpreted as milliseconds.
        await using var db = NewInMemoryContext();
        await SeedAsync(db, DateOptions(new FeatureSpec("PROJ-1", CompletionDate: "2026-05-01")));
        db.GisUploadSourceOrganizations.Single().ApplyCompletedDateToProject = true;
        await db.SaveChangesWithNoAuditingAsync();

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, DateRequest());

        Assert.AreEqual(new DateOnly(2026, 5, 1), (await db.Projects.SingleAsync()).CompletionDate);
    }

    [TestMethod]
    public async Task ImportProjects_TreatsEpochCompletionDateAsPresent_ForStageDerivation()
    {
        // DNR LOA NE derives its stage and is fed by the AGOL job. If an epoch completion date reads
        // as "no completion date", every LOA project silently drops to Planned.
        await using var db = NewInMemoryContext();
        var epochMillis = new DateTimeOffset(new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
        await SeedAsync(db, new SeedOptions
        {
            ProjectStageDefaultID = ProjectStage.Completed.ProjectStageID,
            DataDeriveProjectStage = true,
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", CompletionDate: epochMillis.ToString()) }
        });

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, DateRequest());

        Assert.AreEqual(ProjectStage.Completed.ProjectStageID, (await db.Projects.SingleAsync()).ProjectStageID,
            "An epoch completion date must count as a completion date, or the stage falls back to Planned.");
    }

    [TestMethod]
    public async Task ImportProjects_WidensCompletionDate_ToCoverAnotherProgramsTreatments()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, DateOptions(new FeatureSpec("PROJ-1", CompletionDate: "2026-01-15")));
        db.GisUploadSourceOrganizations.Single().ApplyCompletedDateToProject = true;

        db.Projects.Add(new Project
        {
            ProjectID = 500,
            ProjectName = "Shared Project",
            FhtProjectNumber = "FHT-2026-00001",
            ProjectGisIdentifier = "PROJ-1",
            ProjectTypeID = OtherProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Implementation,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
        });
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = 500, ProgramID = ProgramID });
        db.Treatments.Add(new Treatment
        {
            TreatmentID = 1,
            ProjectID = 500,
            ProgramID = OtherProgramID,
            TreatmentTypeID = TreatmentType.Other.TreatmentTypeID,
            TreatmentDetailedActivityTypeID = TreatmentDetailedActivityType.Other.TreatmentDetailedActivityTypeID,
            TreatmentFootprintAcres = 5m,
            TreatmentEndDate = new DateOnly(2026, 9, 30),
        });
        await db.SaveChangesWithNoAuditingAsync();

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, DateRequest());

        Assert.AreEqual(new DateOnly(2026, 9, 30), (await db.Projects.SingleAsync()).CompletionDate,
            "A project shared across programs must keep a span covering the other program's treatments.");
    }

    [TestMethod]
    public async Task ImportProjects_WidensStartDate_ToCoverAnotherProgramsTreatments()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", CompletionDate: "2026-06-01") }
        });
        var sourceOrg = db.GisUploadSourceOrganizations.Single();
        sourceOrg.ApplyStartDateToProject = true;

        db.Projects.Add(new Project
        {
            ProjectID = 500,
            ProjectName = "Shared Project",
            FhtProjectNumber = "FHT-2026-00001",
            ProjectGisIdentifier = "PROJ-1",
            ProjectTypeID = OtherProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Implementation,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
        });
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = 500, ProgramID = ProgramID });
        db.Treatments.Add(new Treatment
        {
            TreatmentID = 1,
            ProjectID = 500,
            ProgramID = OtherProgramID,
            TreatmentTypeID = TreatmentType.Other.TreatmentTypeID,
            TreatmentDetailedActivityTypeID = TreatmentDetailedActivityType.Other.TreatmentDetailedActivityTypeID,
            TreatmentFootprintAcres = 5m,
            TreatmentStartDate = new DateOnly(2025, 2, 3),
        });
        await db.SaveChangesWithNoAuditingAsync();

        var request = DateRequest();
        request.StartDateMetadataAttributeID = null; // no start date in the GIS data at all
        await GisBulkImports.ImportProjectsAsync(db, AttemptID, request);

        Assert.AreEqual(new DateOnly(2025, 2, 3), (await db.Projects.SingleAsync()).PlannedDate,
            "The start date must reach back to cover the earliest treatment from another program.");
    }

    [TestMethod]
    public async Task ImportProjects_IgnoresSameProgramTreatments_WhenWideningDates()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, DateOptions(new FeatureSpec("PROJ-1", CompletionDate: "2026-01-15")));
        db.GisUploadSourceOrganizations.Single().ApplyCompletedDateToProject = true;

        db.Projects.Add(new Project
        {
            ProjectID = 500,
            ProjectName = "Shared Project",
            FhtProjectNumber = "FHT-2026-00001",
            ProjectGisIdentifier = "PROJ-1",
            ProjectTypeID = OtherProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Implementation,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
        });
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = 500, ProgramID = ProgramID });
        db.Treatments.Add(new Treatment
        {
            TreatmentID = 1,
            ProjectID = 500,
            ProgramID = ProgramID,
            TreatmentTypeID = TreatmentType.Other.TreatmentTypeID,
            TreatmentDetailedActivityTypeID = TreatmentDetailedActivityType.Other.TreatmentDetailedActivityTypeID,
            TreatmentFootprintAcres = 5m,
            TreatmentEndDate = new DateOnly(2026, 9, 30),
        });
        await db.SaveChangesWithNoAuditingAsync();

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, DateRequest());

        Assert.AreEqual(new DateOnly(2026, 1, 15), (await db.Projects.SingleAsync()).CompletionDate,
            "This program's own treatments are being replaced by the import, so they must not widen the span.");
    }

    // ---------------------------------------------------------------------------------------
    // 7. Block list by ProjectID
    // ---------------------------------------------------------------------------------------

    [TestMethod]
    public async Task ImportProjects_SkipsProject_WhenBlockListEntryPointsAtItsProjectID()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1", Name: "Renamed By GIS") }
        });

        db.Projects.Add(new Project
        {
            ProjectID = 500,
            ProjectName = "Do Not Touch",
            FhtProjectNumber = "FHT-2026-00001",
            ProjectGisIdentifier = "PROJ-1",
            ProjectTypeID = OtherProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Implementation,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
        });
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = 500, ProgramID = ProgramID });
        db.ProjectImportBlockLists.Add(new ProjectImportBlockList
        {
            ProjectImportBlockListID = 1,
            ProgramID = ProgramID,
            ProjectID = 500,
        });
        await db.SaveChangesWithNoAuditingAsync();

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsBlocked);
        Assert.AreEqual(0, result.ProjectsUpdated);
        Assert.AreEqual(0, result.ProjectsCreated);

        db.ChangeTracker.Clear();
        var project = await db.Projects.SingleAsync();
        Assert.AreEqual("Do Not Touch", project.ProjectName, "A blocked project must not be renamed from the GIS data.");
        Assert.AreEqual(0, await db.ProjectLocations.CountAsync(), "A blocked project must not get imported locations.");
    }

    [TestMethod]
    public async Task ImportProjects_DoesNotBlock_WhenBlockListEntryIsForAnotherProgram()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, new SeedOptions
        {
            ImportAsDetailedLocationInsteadOfTreatments = true,
            Features = { new FeatureSpec("PROJ-1") }
        });

        db.Projects.Add(new Project
        {
            ProjectID = 500,
            ProjectName = "Existing",
            FhtProjectNumber = "FHT-2026-00001",
            ProjectGisIdentifier = "PROJ-1",
            ProjectTypeID = OtherProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Implementation,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
        });
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = 500, ProgramID = ProgramID });
        db.ProjectImportBlockLists.Add(new ProjectImportBlockList
        {
            ProjectImportBlockListID = 1,
            ProgramID = OtherProgramID,
            ProjectID = 500,
        });
        await db.SaveChangesWithNoAuditingAsync();

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(0, result.ProjectsBlocked);
        Assert.AreEqual(1, result.ProjectsUpdated);
    }
}
