using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using WADNR.API.Services;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Tests;

/// <summary>
/// Covers WADNR-2150 rework: the LOA / Service Forestry import must carry the source Esri
/// OBJECTID / GlobalID through onto each created ProjectLocation (ArcGisObjectID / ArcGisGlobalID),
/// which the GDB download's ProjectLocations layer then surfaces. Previously these columns were
/// never set and came out blank for every LOA record. Also covers WADNR-2272: imported projects must
/// get County / DNR Upland Region / Priority Landscape populated from their location geometry.
///
/// These run against a real SQL Server database. They used to use the InMemory provider, which was
/// never faithful here and became untenable when region assignment moved to set-based SQL:
///
///   - InMemory has no relational provider, so <c>ExecuteSqlRaw</c> throws. Region assignment is now
///     raw SQL, and the treatment import proc already needed a real connection — that failure was
///     simply swallowed by the catch that turns proc errors into warnings, so every one of these tests
///     had been passing with the treatment import silently skipped.
///   - InMemory evaluates <c>Intersects</c> with in-process NetTopologySuite, which is not the engine
///     SQL Server uses. A spatial assertion that passes there proves nothing about production.
///
/// The two region tests are correspondingly stronger now: instead of seeding a fake county that
/// trivially overlaps, they compare what the import assigned against what a direct spatial query says
/// should have been assigned, using the real WA boundary data.
/// </summary>
[TestClass]
public class GisBulkImportArcGisIdTests
{
    private const string Identifier = "PROJ-1";

    private int _programID;
    private int _sourceOrganizationID;
    private int _attemptID;

    [TestCleanup]
    public async Task TearDown()
    {
        if (_attemptID == 0)
        {
            return;
        }

        await using var dbContext = NewContext();
        await TearDownFixtureAsync(dbContext);
        _attemptID = 0;
    }

    #region ArcGIS identifier carry-through

    [TestMethod]
    public async Task ImportProjects_SetsArcGisObjectIDAndGlobalID_WhenMetadataPresent()
    {
        await using var db = NewContext();
        var request = await ArrangeAsync(db, objectIdValue: "4242", globalIdValue: "{ABC-123}");

        var result = await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        Assert.AreEqual(1, result.ProjectsCreated);
        var location = await SingleImportedLocationAsync(db);
        Assert.AreEqual(4242, location.ArcGisObjectID);
        Assert.AreEqual("{ABC-123}", location.ArcGisGlobalID);
    }

    [TestMethod]
    public async Task ImportProjects_LeavesArcGisColumnsNull_WhenMetadataAbsent()
    {
        await using var db = NewContext();
        var request = await ArrangeAsync(db, objectIdValue: null, globalIdValue: null);

        var result = await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        Assert.AreEqual(1, result.ProjectsCreated);
        var location = await SingleImportedLocationAsync(db);
        Assert.IsNull(location.ArcGisObjectID);
        Assert.IsNull(location.ArcGisGlobalID);
    }

    [TestMethod]
    public async Task ImportProjects_DoesNotThrowAndLeavesObjectIDNull_WhenObjectIdUnparseable()
    {
        await using var db = NewContext();
        var request = await ArrangeAsync(db, objectIdValue: "not-a-number", globalIdValue: "{XYZ}");

        var result = await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        Assert.AreEqual(1, result.ProjectsCreated);
        var location = await SingleImportedLocationAsync(db);
        Assert.IsNull(location.ArcGisObjectID);
        Assert.AreEqual("{XYZ}", location.ArcGisGlobalID);
    }

    #endregion

    #region Location naming (WADNR-2150)

    [TestMethod]
    public async Task ImportProjects_NamesLocationFromObjectID_WhenObjectIdPresent()
    {
        // The location name must derive from the stable Esri OBJECTID, not the positional
        // GisImportFeatureKey, so re-imports produce identical names and cross-feature/cross-program
        // name collisions on AK_ProjectLocation_ProjectID_ProgramID_ProjectLocationName are avoided.
        await using var db = NewContext();
        var request = await ArrangeAsync(db, objectIdValue: "4242", globalIdValue: "{ABC-123}");

        await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        var location = await SingleImportedLocationAsync(db);
        Assert.AreEqual($"{Identifier} - Feature 4242", location.ProjectLocationName);
    }

    [TestMethod]
    public async Task ImportProjects_NamesLocationFromGlobalID_WhenObjectIdAbsent()
    {
        await using var db = NewContext();
        var request = await ArrangeAsync(db, objectIdValue: null, globalIdValue: "{ABC-123}");

        await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        var location = await SingleImportedLocationAsync(db);
        Assert.AreEqual($"{Identifier} - Feature {{ABC-123}}", location.ProjectLocationName);
    }

    [TestMethod]
    public async Task ImportProjects_NamesLocationFromFeatureKey_WhenNoEsriIdentifiers()
    {
        // Non-Esri sources carry neither OBJECTID nor GlobalID; fall back to the feature key.
        await using var db = NewContext();
        var request = await ArrangeAsync(db, objectIdValue: null, globalIdValue: null);

        await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        var location = await SingleImportedLocationAsync(db);
        Assert.AreEqual($"{Identifier} - Feature 0", location.ProjectLocationName);
    }

    #endregion

    #region Geographic region assignment (WADNR-2272)

    [TestMethod]
    public async Task ImportProjects_AssignsCountyRegionAndPriorityLandscape_FromLocationGeometry()
    {
        // WADNR-2272: imported projects were not getting location-based data set. Asserted against
        // ground truth computed by an independent spatial query over the real boundary tables, rather
        // than against a seeded polygon that trivially overlaps — so this now verifies the import agrees
        // with SQL Server's spatial engine on real Washington data.
        await using var db = NewContext();
        var request = await ArrangeAsync(db, objectIdValue: "4242", globalIdValue: "{ABC-123}", geometry: InsideWashington());

        var result = await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        Assert.AreEqual(1, result.ProjectsCreated);
        var projectID = (await ImportedProjectsAsync(db)).Single();

        var expected = await ExpectedRegionsAsync(db, InsideWashington());
        Assert.IsTrue(expected.Counties > 0,
            "Fixture geometry no longer intersects any County, so this test would pass vacuously.");

        Assert.AreEqual(expected.Counties, await db.ProjectCounties.CountAsync(x => x.ProjectID == projectID),
            "Assigned counties should match what a direct spatial query returns.");
        Assert.AreEqual(expected.Regions, await db.ProjectRegions.CountAsync(x => x.ProjectID == projectID),
            "Assigned DNR upland regions should match what a direct spatial query returns.");
        Assert.AreEqual(expected.PriorityLandscapes, await db.ProjectPriorityLandscapes.CountAsync(x => x.ProjectID == projectID),
            "Assigned priority landscapes should match what a direct spatial query returns.");
    }

    [TestMethod]
    public async Task ImportProjects_LeavesLocationDataEmptyAndSetsExplanations_WhenNoBoundaryIntersects()
    {
        // Geometry far outside Washington, so nothing intersects: the import must still succeed, leave
        // the join tables empty, and record the "does not intersect" explanations.
        await using var db = NewContext();
        var request = await ArrangeAsync(db, objectIdValue: "4242", globalIdValue: "{ABC-123}", geometry: FarFromWashington());

        var result = await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        Assert.AreEqual(1, result.ProjectsCreated);
        var projectID = (await ImportedProjectsAsync(db)).Single();

        Assert.AreEqual(0, await db.ProjectCounties.CountAsync(x => x.ProjectID == projectID));
        Assert.AreEqual(0, await db.ProjectRegions.CountAsync(x => x.ProjectID == projectID));
        Assert.AreEqual(0, await db.ProjectPriorityLandscapes.CountAsync(x => x.ProjectID == projectID));

        db.ChangeTracker.Clear();
        var project = await db.Projects.AsNoTracking().SingleAsync(p => p.ProjectID == projectID);
        Assert.IsNotNull(project.NoCountiesExplanation);
        Assert.IsNotNull(project.NoRegionsExplanation);
        Assert.IsNotNull(project.NoPriorityLandscapesExplanation);
    }

    [TestMethod]
    public async Task ImportProjects_AssignsEachProjectItsOwnRegions_WhenImportingSeveralAtOnce()
    {
        // Region assignment is one set-based SQL batch over every project in the import: temp tables,
        // an OPENJSON project list, and a CROSS JOIN against the boundary tables reduced with
        // DISTINCT g.ProjectID. If that per-project scoping were wrong — a join dropped, the DISTINCT
        // widened — every project in the batch would receive the union of all their regions, and a
        // single-project fixture could never tell. So: two projects at opposite ends of the state,
        // asserted by ID against what an independent spatial query returns for each one's own
        // geometry.
        await using var db = NewContext();
        var eastern = EasternWashington();
        var northwestern = NorthwesternWashington();
        var request = await ArrangeMultiAsync(db, ("PROJ-EAST", eastern), ("PROJ-NW", northwestern));

        var expectedEasternCounties = await ExpectedCountyIDsAsync(db, eastern);
        var expectedNorthwesternCounties = await ExpectedCountyIDsAsync(db, northwestern);

        // Fixture guards: without these the disjointness assertions below could pass vacuously.
        Assert.IsTrue(expectedEasternCounties.Count > 0 && expectedNorthwesternCounties.Count > 0,
            "Both fixture geometries must intersect a County, or this test proves nothing.");
        Assert.AreEqual(0, expectedEasternCounties.Intersect(expectedNorthwesternCounties).Count(),
            "The two fixture geometries must sit in different counties for cross-contamination to be detectable.");

        var result = await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        Assert.AreEqual(2, result.ProjectsCreated);

        var projectIDByIdentifier = await ProjectIDByGisIdentifierAsync(db);

        CollectionAssert.AreEquivalent(
            expectedEasternCounties,
            await AssignedCountyIDsAsync(db, projectIDByIdentifier["PROJ-EAST"]),
            "The eastern project must get exactly its own counties — not the other project's as well.");
        CollectionAssert.AreEquivalent(
            expectedNorthwesternCounties,
            await AssignedCountyIDsAsync(db, projectIDByIdentifier["PROJ-NW"]),
            "The northwestern project must get exactly its own counties — not the other project's as well.");

        // Same check one level up: DNR upland regions are assigned by the same batch and the same join.
        CollectionAssert.AreEquivalent(
            await ExpectedRegionIDsAsync(db, eastern),
            await AssignedRegionIDsAsync(db, projectIDByIdentifier["PROJ-EAST"]));
        CollectionAssert.AreEquivalent(
            await ExpectedRegionIDsAsync(db, northwestern),
            await AssignedRegionIDsAsync(db, projectIDByIdentifier["PROJ-NW"]));
    }

    [TestMethod]
    public async Task ImportProjects_ReplacesLocationsAndRegions_WhenSeveralProjectsAreReImported()
    {
        // The prior-location cleanup is now one SELECT plus two deletes for the whole import rather
        // than three statements per project, and region assignment likewise deletes and re-inserts in
        // one batch. Re-importing the same features must therefore leave exactly what the first run
        // left — no duplicated locations, no stacked region rows — across more than one project.
        await using var db = NewContext();
        var eastern = EasternWashington();
        var northwestern = NorthwesternWashington();
        var features = new[] { ("PROJ-EAST", eastern), ("PROJ-NW", northwestern) };
        var request = await ArrangeMultiAsync(db, features);

        var firstRun = await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);
        Assert.AreEqual(2, firstRun.ProjectsCreated);
        Assert.AreEqual(2, firstRun.LocationsCreated);

        var projectIDByIdentifier = await ProjectIDByGisIdentifierAsync(db);
        var easternCountiesAfterFirstRun = await AssignedCountyIDsAsync(db, projectIDByIdentifier["PROJ-EAST"]);

        // A successful import clears its own staged features, so re-stage before running again.
        db.ChangeTracker.Clear();
        await StageFeaturesAsync(db, features);

        var secondRun = await GisBulkImports.ImportProjectsAsync(db, _attemptID, request);

        Assert.AreEqual(0, secondRun.ProjectsCreated,
            "Both projects already exist under this program, so the second run must match rather than duplicate them.");
        Assert.AreEqual(2, secondRun.ProjectsUpdated);

        db.ChangeTracker.Clear();
        Assert.AreEqual(2, (await ImportedProjectsAsync(db)).Count,
            "Re-importing must not create a second project per identifier.");

        var projectIDs = await ImportedProjectsAsync(db);
        Assert.AreEqual(2, await db.ProjectLocations
                .CountAsync(l => projectIDs.Contains(l.ProjectID) && l.ImportedFromGisUpload == true),
            "The batched delete must clear the first run's project areas before the second run re-creates them.");

        CollectionAssert.AreEquivalent(
            easternCountiesAfterFirstRun,
            await AssignedCountyIDsAsync(db, projectIDByIdentifier["PROJ-EAST"]),
            "Region assignment deletes and re-inserts, so a re-import must land on the same set, not a doubled one.");
    }

    #endregion

    #region BuildOutFields (no database)

    [TestMethod]
    public void BuildOutFields_AppendsObjectIdAndGlobalId_WhenIncludeGlobalIdTrue()
    {
        var outFields = GisDataImportService.BuildOutFields(new[] { "Approval_ID", "Project_Name" }, includeGlobalId: true);

        var fields = outFields.Split(',');
        CollectionAssert.Contains(fields, "Approval_ID");
        CollectionAssert.Contains(fields, "Project_Name");
        CollectionAssert.Contains(fields, "OBJECTID");
        CollectionAssert.Contains(fields, "GlobalID");
    }

    [TestMethod]
    public void BuildOutFields_AppendsObjectIdOnly_WhenIncludeGlobalIdFalse()
    {
        // USFS / NEPA boundary services have no GlobalID field — it must not be requested.
        var outFields = GisDataImportService.BuildOutFields(new[] { "ACTIVITY_CODE" }, includeGlobalId: false);

        var fields = outFields.Split(',');
        CollectionAssert.Contains(fields, "ACTIVITY_CODE");
        CollectionAssert.Contains(fields, "OBJECTID");
        Assert.IsFalse(fields.Any(f => string.Equals(f, "GlobalID", StringComparison.OrdinalIgnoreCase)),
            "GlobalID must not be requested when includeGlobalId is false.");
    }

    [TestMethod]
    public void BuildOutFields_DoesNotDuplicateEsriSystemFields_WhenAlreadyMappedCaseInsensitively()
    {
        var outFields = GisDataImportService.BuildOutFields(new[] { "objectid", "globalid", "Approval_ID" }, includeGlobalId: true);

        var fields = outFields.Split(',');
        Assert.AreEqual(1, fields.Count(f => string.Equals(f, "objectid", StringComparison.OrdinalIgnoreCase)));
        Assert.AreEqual(1, fields.Count(f => string.Equals(f, "globalid", StringComparison.OrdinalIgnoreCase)));
    }

    #endregion

    #region Fixture

    private static WADNRDbContext NewContext()
    {
        var connectionString = AssemblySteps.Configuration["sqlConnectionString"]
            ?? throw new InvalidOperationException("sqlConnectionString not found in environment.json");

        var options = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseSqlServer(connectionString, x =>
            {
                x.UseNetTopologySuite();
                x.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
            })
            .Options;

        return new WADNRDbContext(options, AssemblySteps.AuditUserProvider);
    }

    /// <summary>Eastern Washington, so it intersects real County / DNRUplandRegion geometry.</summary>
    private static Geometry InsideWashington() => Square(-120.0, 47.0);

    /// <summary>Mid-Atlantic, so nothing in the WA boundary tables can intersect it.</summary>
    private static Geometry FarFromWashington() => Square(-40.0, 30.0);

    /// <summary>
    /// Two squares roughly 300km apart at opposite ends of the state, so they fall in different
    /// counties and different DNR upland regions. The multi-project tests rely on that separation to
    /// detect a batch that assigns one project's regions to another.
    /// </summary>
    private static Geometry EasternWashington() => Square(-118.5, 46.4);

    private static Geometry NorthwesternWashington() => Square(-122.4, 48.4);

    private static Geometry Square(double minX, double minY)
    {
        const double size = 0.1;
        var factory = new GeometryFactory(new PrecisionModel(), 4326);
        return factory.CreatePolygon(factory.CreateLinearRing(
        [
            new Coordinate(minX, minY),
            new Coordinate(minX, minY + size),
            new Coordinate(minX + size, minY + size),
            new Coordinate(minX + size, minY),
            new Coordinate(minX, minY)
        ]));
    }

    /// <summary>
    /// Seeds Program + source org + attempt, stages one feature through the real upload path, and
    /// returns the import request.
    ///
    /// Staging via <c>UploadAndProcessFileAsync</c> rather than hand-inserting GisFeature rows is what
    /// creates the GisMetadataAttribute and GisUploadAttemptGisMetadataAttribute records the import
    /// joins through, and it keeps the fixture honest about what production actually produces.
    /// </summary>
    private async Task<GisBulkImportRequest> ArrangeAsync(
        WADNRDbContext dbContext, string? objectIdValue, string? globalIdValue, Geometry? geometry = null)
    {
        await SeedFixtureAsync(dbContext);

        await GisBulkImports.UploadAndProcessFileAsync(
            dbContext, _attemptID, BuildGeoJson(objectIdValue, globalIdValue, geometry ?? InsideWashington()));

        return await BuildRequestAsync(dbContext);
    }

    /// <summary>
    /// Stages several features, one per project identifier, through the same real upload path.
    /// Used by the multi-project tests: the import is set-based, so a single-project fixture leaves
    /// the batching — and the region SQL's per-project scoping in particular — unexercised.
    /// </summary>
    private async Task<GisBulkImportRequest> ArrangeMultiAsync(
        WADNRDbContext dbContext, params (string Identifier, Geometry Geometry)[] features)
    {
        await SeedFixtureAsync(dbContext);
        await StageFeaturesAsync(dbContext, features);
        return await BuildRequestAsync(dbContext);
    }

    /// <summary>One feature per project identifier, each with its own geometry.</summary>
    private static string BuildMultiFeatureGeoJson((string Identifier, Geometry Geometry)[] features)
    {
        var featureCollection = new FeatureCollection();
        foreach (var (identifier, geometry) in features)
        {
            featureCollection.Add(new Feature(geometry, new AttributesTable
            {
                { "approval_id", identifier },
                { "project_name", $"Test Project {identifier}" }
            }));
        }

        var options = new JsonSerializerOptions();
        options.Converters.Add(new GeoJsonConverterFactory());
        return JsonSerializer.Serialize(featureCollection, options);
    }

    /// <summary>
    /// Stages the given features onto this attempt. UploadAndProcessFileAsync clears the attempt's
    /// prior staged rows first, so calling this a second time is exactly what a repeat upload of the
    /// same GDB does — and it is the only way to exercise a second import, because a successful one
    /// clears its own staged features.
    /// </summary>
    private async Task StageFeaturesAsync(
        WADNRDbContext dbContext, params (string Identifier, Geometry Geometry)[] features) =>
        await GisBulkImports.UploadAndProcessFileAsync(
            dbContext, _attemptID, BuildMultiFeatureGeoJson(features));

    private async Task<GisBulkImportRequest> BuildRequestAsync(WADNRDbContext dbContext)
    {
        var attributeIDByName = await dbContext.GisUploadAttemptGisMetadataAttributes
            .AsNoTracking()
            .Where(x => x.GisUploadAttemptID == _attemptID)
            .Select(x => new { x.GisMetadataAttributeID, x.GisMetadataAttribute.GisMetadataAttributeName })
            .ToDictionaryAsync(x => x.GisMetadataAttributeName, x => x.GisMetadataAttributeID, StringComparer.OrdinalIgnoreCase);

        return new GisBulkImportRequest
        {
            ProjectIdentifierMetadataAttributeID = attributeIDByName["approval_id"],
            ProjectNameMetadataAttributeID = attributeIDByName["project_name"]
        };
    }

    private async Task SeedFixtureAsync(WADNRDbContext dbContext)
    {
        var program = await ProgramHelper.CreateProgramAsync(
            dbContext, AssemblySteps.TestAdminPersonID, name: $"ArcGis Id Test {DateTime.UtcNow:yyyyMMddHHmmssfff}");
        _programID = program.ProgramID;

        var organizationID = (await dbContext.Organizations.AsNoTracking().OrderBy(x => x.OrganizationID).FirstAsync()).OrganizationID;
        var relationshipTypeID = (await dbContext.RelationshipTypes.AsNoTracking().OrderBy(x => x.RelationshipTypeID).FirstAsync()).RelationshipTypeID;
        var projectTypeName = (await dbContext.ProjectTypes.AsNoTracking().OrderBy(x => x.ProjectTypeID).FirstAsync()).ProjectTypeName;

        var sourceOrganization = new GisUploadSourceOrganization
        {
            GisUploadSourceOrganizationName = $"ArcGis Id Test Source {program.ProgramID}",
            ProgramID = program.ProgramID,
            ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
            ProjectTypeDefaultName = projectTypeName,
            DefaultLeadImplementerOrganizationID = organizationID,
            RelationshipTypeForDefaultOrganizationID = relationshipTypeID,
            ApplyStartDateToProject = false,
            ApplyCompletedDateToProject = false,
            ImportIsFlattened = false
        };
        dbContext.GisUploadSourceOrganizations.Add(sourceOrganization);
        await dbContext.SaveChangesWithNoAuditingAsync();
        _sourceOrganizationID = sourceOrganization.GisUploadSourceOrganizationID;

        var attempt = new GisUploadAttempt
        {
            GisUploadSourceOrganizationID = sourceOrganization.GisUploadSourceOrganizationID,
            GisUploadAttemptCreatePersonID = AssemblySteps.TestAdminPersonID,
            GisUploadAttemptCreateDate = new DateTime(2026, 6, 16, 0, 0, 0, DateTimeKind.Utc)
        };
        dbContext.GisUploadAttempts.Add(attempt);
        await dbContext.SaveChangesWithNoAuditingAsync();
        _attemptID = attempt.GisUploadAttemptID;
    }

    private static string BuildGeoJson(string? objectIdValue, string? globalIdValue, Geometry geometry)
    {
        var attributes = new AttributesTable
        {
            { "approval_id", Identifier },
            { "project_name", "Test LOA Project" }
        };
        if (objectIdValue != null)
        {
            attributes.Add("objectid", objectIdValue);
        }
        if (globalIdValue != null)
        {
            attributes.Add("globalid", globalIdValue);
        }

        var featureCollection = new FeatureCollection { new Feature(geometry, attributes) };

        var options = new JsonSerializerOptions();
        options.Converters.Add(new GeoJsonConverterFactory());
        return JsonSerializer.Serialize(featureCollection, options);
    }

    /// <summary>What a direct spatial query says should intersect, independent of the import.</summary>
    private static async Task<(int Counties, int Regions, int PriorityLandscapes)> ExpectedRegionsAsync(
        WADNRDbContext dbContext, Geometry geometry) =>
    (
        await dbContext.Counties.AsNoTracking().CountAsync(c => c.CountyFeature.Intersects(geometry)),
        await dbContext.DNRUplandRegions.AsNoTracking().CountAsync(r => r.DNRUplandRegionLocation.Intersects(geometry)),
        await dbContext.PriorityLandscapes.AsNoTracking().CountAsync(p => p.PriorityLandscapeLocation.Intersects(geometry))
    );

    /// <summary>Which County / DNRUplandRegion rows a direct spatial query says a geometry hits.</summary>
    private static async Task<List<int>> ExpectedCountyIDsAsync(WADNRDbContext dbContext, Geometry geometry) =>
        await dbContext.Counties.AsNoTracking()
            .Where(c => c.CountyFeature.Intersects(geometry)).Select(c => c.CountyID).ToListAsync();

    private static async Task<List<int>> ExpectedRegionIDsAsync(WADNRDbContext dbContext, Geometry geometry) =>
        await dbContext.DNRUplandRegions.AsNoTracking()
            .Where(r => r.DNRUplandRegionLocation.Intersects(geometry)).Select(r => r.DNRUplandRegionID).ToListAsync();

    /// <summary>What the import actually assigned.</summary>
    private static async Task<List<int>> AssignedCountyIDsAsync(WADNRDbContext dbContext, int projectID) =>
        await dbContext.ProjectCounties.AsNoTracking()
            .Where(x => x.ProjectID == projectID).Select(x => x.CountyID).ToListAsync();

    private static async Task<List<int>> AssignedRegionIDsAsync(WADNRDbContext dbContext, int projectID) =>
        await dbContext.ProjectRegions.AsNoTracking()
            .Where(x => x.ProjectID == projectID).Select(x => x.DNRUplandRegionID).ToListAsync();

    private async Task<Dictionary<string, int>> ProjectIDByGisIdentifierAsync(WADNRDbContext dbContext) =>
        await dbContext.Projects
            .AsNoTracking()
            .Where(p => (p.CreateGisUploadAttemptID == _attemptID || p.LastUpdateGisUploadAttemptID == _attemptID)
                && p.ProjectGisIdentifier != null)
            .ToDictionaryAsync(p => p.ProjectGisIdentifier!, p => p.ProjectID);

    private async Task<List<int>> ImportedProjectsAsync(WADNRDbContext dbContext) =>
        await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.CreateGisUploadAttemptID == _attemptID || p.LastUpdateGisUploadAttemptID == _attemptID)
            .Select(p => p.ProjectID)
            .ToListAsync();

    /// <summary>
    /// The single location the import created. Filtered on ImportedFromGisUpload because
    /// procImportTreatmentsFromGisUploadAttempt creates its own ProjectLocation rows, which now really
    /// run — under the InMemory provider they never did.
    /// </summary>
    private async Task<ProjectLocation> SingleImportedLocationAsync(WADNRDbContext dbContext)
    {
        var projectIDs = await ImportedProjectsAsync(dbContext);
        return await dbContext.ProjectLocations
            .AsNoTracking()
            .SingleAsync(l => projectIDs.Contains(l.ProjectID) && l.ImportedFromGisUpload == true);
    }

    private async Task TearDownFixtureAsync(WADNRDbContext dbContext)
    {
        dbContext.ChangeTracker.Clear();

        var projectIDs = await ImportedProjectsAsync(dbContext);
        if (projectIDs.Count > 0)
        {
            await dbContext.Treatments.Where(x => projectIDs.Contains(x.ProjectID)).ExecuteDeleteAsync();
            await dbContext.ProjectLocations.Where(x => projectIDs.Contains(x.ProjectID)).ExecuteDeleteAsync();
            await dbContext.ProjectCounties.Where(x => projectIDs.Contains(x.ProjectID)).ExecuteDeleteAsync();
            await dbContext.ProjectRegions.Where(x => projectIDs.Contains(x.ProjectID)).ExecuteDeleteAsync();
            await dbContext.ProjectPriorityLandscapes.Where(x => projectIDs.Contains(x.ProjectID)).ExecuteDeleteAsync();
            await dbContext.ProjectPrograms.Where(x => projectIDs.Contains(x.ProjectID)).ExecuteDeleteAsync();
            await dbContext.ProjectOrganizations.Where(x => projectIDs.Contains(x.ProjectID)).ExecuteDeleteAsync();
            await dbContext.Projects.Where(x => projectIDs.Contains(x.ProjectID)).ExecuteDeleteAsync();
        }

        var featureIDs = dbContext.GisFeatures.Where(f => f.GisUploadAttemptID == _attemptID).Select(f => f.GisFeatureID);
        await dbContext.GisFeatureMetadataAttributes.Where(x => featureIDs.Contains(x.GisFeatureID)).ExecuteDeleteAsync();
        await dbContext.GisFeatures.Where(x => x.GisUploadAttemptID == _attemptID).ExecuteDeleteAsync();
        await dbContext.GisUploadAttemptGisMetadataAttributes.Where(x => x.GisUploadAttemptID == _attemptID).ExecuteDeleteAsync();
        await dbContext.GisUploadAttempts.Where(x => x.GisUploadAttemptID == _attemptID).ExecuteDeleteAsync();

        if (_sourceOrganizationID != 0)
        {
            await dbContext.GisUploadSourceOrganizations
                .Where(x => x.GisUploadSourceOrganizationID == _sourceOrganizationID).ExecuteDeleteAsync();
        }

        if (_programID != 0)
        {
            await dbContext.Programs.Where(x => x.ProgramID == _programID).ExecuteDeleteAsync();
        }

        // GisMetadataAttribute rows are shared global data the production code only ever adds to.
        dbContext.ChangeTracker.Clear();
    }

    #endregion
}
