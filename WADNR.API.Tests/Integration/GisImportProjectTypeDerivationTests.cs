using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// End-to-end cover for WADNR-2287 against a real SQL Server database, configured the way
/// DNR State Lands (ProgramID 1) is in production: no ProjectTypeDefaultName,
/// AdjustProjectTypeBasedOnTreatmentTypes on, DataDeriveProjectStage on, and crosswalks for
/// treatment type, project stage and lead implementer.
///
/// This is the piece the in-memory tests cannot reach. The project-type derivation runs only after
/// dbo.procImportTreatmentsFromGisUploadAttempt has actually produced treatments, and the proc is
/// also what resolves each treatment's TreatmentTypeID through the FieldDefinition 468 crosswalk.
/// On the InMemory provider the proc call throws, so the derivation is skipped by design and any
/// assertion about it would be vacuous.
///
/// The production symptom this reproduces: 442 projects created by attempts 6011 / 6015 all landed
/// on "Research and Monitoring" — the lowest ProjectTypeID in dbo.ProjectType, which is what the old
/// `ProjectTypes.First()` fallback selected — instead of being typed from their treatments.
/// </summary>
[TestClass]
public class GisImportProjectTypeDerivationTests
{
    private const string OtherProjectTypeName = "Other";
    private const string NonCommercialProjectTypeName = "Non-commercial vegetation treatment";
    private const string PrescribedFireProjectTypeName = "Prescribed fire treatment";

    // Source values invented for this fixture so they cannot collide with a real crosswalk row.
    private const string ThinningSourceValue = "WADNR2287-THIN";
    private const string BurnSourceValue = "WADNR2287-BURN";
    private const string UnmappedSourceValue = "WADNR2287-UNMAPPED";
    private const string ProjectStageSourceValue = "WADNR2287-DONE";
    private const string LeadImplementerSourceValue = "WADNR2287-IMPLEMENTER";

    private const int ThinningFeatureIdentifier = 9_200_000;
    private const int BurnFeatureIdentifier = 9_200_001;
    private const int UnmappedFeatureIdentifier = 9_200_002;

    private int _programID;
    private int _sourceOrganizationID;
    private int _attemptID;
    private int _crosswalkedOrganizationID;

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

    #region Tests

    [TestMethod]
    public async Task ImportProjects_TypesProjectsFromTheirTreatments_NotFromTheLowestIDProjectType()
    {
        await using var dbContext = NewContext();
        var request = await ArrangeAsync(dbContext);

        var lowestIDProjectTypeName = (await dbContext.ProjectTypes
            .AsNoTracking().OrderBy(x => x.ProjectTypeID).FirstAsync()).ProjectTypeName;

        var result = await GisBulkImports.ImportProjectsAsync(dbContext, _attemptID, request);

        Assert.AreEqual(0, result.Warnings.Count,
            $"The treatment import must succeed, or the derivation is skipped by design: {string.Join(" | ", result.Warnings)}");
        Assert.AreEqual(3, result.ProjectsCreated);

        var projectTypeNameByIdentifier = await ProjectTypeNameByGisIdentifierAsync(dbContext);

        Assert.AreEqual(NonCommercialProjectTypeName, projectTypeNameByIdentifier[ThinningFeatureIdentifier.ToString()],
            "A project whose only treatment is Non-Commercial must be typed from that treatment.");
        Assert.AreEqual(PrescribedFireProjectTypeName, projectTypeNameByIdentifier[BurnFeatureIdentifier.ToString()],
            "A project whose only treatment is Prescribed Fire must be typed from that treatment.");
        Assert.AreEqual(OtherProjectTypeName, projectTypeNameByIdentifier[UnmappedFeatureIdentifier.ToString()],
            "A project whose treatment type is Other keeps the creation fallback, which must be \"Other\".");

        // The regression guard: in production the lowest ProjectTypeID is "Research and Monitoring",
        // which is exactly what every one of these projects used to be assigned.
        CollectionAssert.DoesNotContain(projectTypeNameByIdentifier.Values.ToList(), lowestIDProjectTypeName,
            $"No imported project may fall back to the lowest-ID project type (\"{lowestIDProjectTypeName}\").");
    }

    [TestMethod]
    public async Task ImportProjects_AppliesProjectStageAndLeadImplementerCrosswalks()
    {
        await using var dbContext = NewContext();
        var request = await ArrangeAsync(dbContext);

        var result = await GisBulkImports.ImportProjectsAsync(dbContext, _attemptID, request);

        Assert.AreEqual(0, result.Warnings.Count, string.Join(" | ", result.Warnings));

        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.CreateGisUploadAttemptID == _attemptID)
            .Select(p => new { p.ProjectID, p.ProjectStageID })
            .ToListAsync();

        Assert.AreEqual(3, projects.Count);
        Assert.IsTrue(projects.All(p => p.ProjectStageID == ProjectStage.Completed.ProjectStageID),
            "DataDeriveProjectStage plus a crosswalk row mapping the source value to \"Completed\" must win " +
            "over the source org's configured ProjectStageDefaultID (Implementation).");

        var projectIDs = projects.Select(p => p.ProjectID).ToList();
        var organizationIDs = await dbContext.ProjectOrganizations
            .AsNoTracking()
            .Where(x => projectIDs.Contains(x.ProjectID))
            .Select(x => x.OrganizationID)
            .Distinct()
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { _crosswalkedOrganizationID }, organizationIDs,
            "The LeadImplementer crosswalk must resolve the organization, not the source org's default.");
    }

    #endregion

    #region Fixture

    private static WADNRDbContext NewContext()
    {
        var connectionString = AssemblySteps.Configuration["sqlConnectionString"]
            ?? throw new InvalidOperationException("sqlConnectionString not found in environment.json");

        var builder = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseSqlServer(connectionString, x =>
            {
                x.UseNetTopologySuite();
                x.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
            });

        return new WADNRDbContext(builder.Options, AssemblySteps.AuditUserProvider);
    }

    /// <summary>
    /// Seeds a source organization mirroring DNR State Lands' production configuration, plus the
    /// crosswalk rows the treatment proc and the import both read, then stages features through the
    /// real upload path.
    /// </summary>
    private async Task<GisBulkImportRequest> ArrangeAsync(WADNRDbContext dbContext)
    {
        var program = await ProgramHelper.CreateProgramAsync(
            dbContext, AssemblySteps.TestAdminPersonID, name: $"GIS Project Type Derivation Test {DateTime.UtcNow:yyyyMMddHHmmssfff}");
        _programID = program.ProgramID;

        var organizations = await dbContext.Organizations
            .AsNoTracking().OrderBy(x => x.OrganizationID).Take(2)
            .Select(x => new { x.OrganizationID, x.OrganizationName })
            .ToListAsync();
        Assert.AreEqual(2, organizations.Count, "This fixture needs at least two organizations to tell default from crosswalked.");
        var defaultOrganizationID = organizations[0].OrganizationID;
        _crosswalkedOrganizationID = organizations[1].OrganizationID;
        var crosswalkedOrganizationName = organizations[1].OrganizationName;

        var relationshipTypeID = (await dbContext.RelationshipTypes.AsNoTracking().OrderBy(x => x.RelationshipTypeID).FirstAsync()).RelationshipTypeID;

        // Everything the production DNR State Lands source org (ProgramID 1) has set, which is what
        // made it hit the bad fallback: no ProjectTypeDefaultName, derive the type from treatments.
        var sourceOrganization = new GisUploadSourceOrganization
        {
            GisUploadSourceOrganizationName = $"Project Type Derivation Source {program.ProgramID}",
            ProgramID = program.ProgramID,
            ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
            ProjectTypeDefaultName = null,
            TreatmentTypeDefaultName = null,
            DefaultLeadImplementerOrganizationID = defaultOrganizationID,
            RelationshipTypeForDefaultOrganizationID = relationshipTypeID,
            AdjustProjectTypeBasedOnTreatmentTypes = true,
            DataDeriveProjectStage = true,
            ImportAsDetailedLocationInsteadOfTreatments = false,
            ImportAsDetailedLocationInAdditionToTreatments = false,
            ApplyStartDateToProject = true,
            ApplyCompletedDateToProject = true,
            ApplyStartDateToTreatments = true,
            ApplyEndDateToTreatments = true,
            ImportIsFlattened = false,
            ProjectDescriptionDefaultText = "Created by GisImportProjectTypeDerivationTests."
        };
        dbContext.GisUploadSourceOrganizations.Add(sourceOrganization);
        await dbContext.SaveChangesWithNoAuditingAsync();
        _sourceOrganizationID = sourceOrganization.GisUploadSourceOrganizationID;

        // FieldDefinition 468 (TreatmentType) is read by the proc, which maps the crosswalked value
        // onto TreatmentType.TreatmentTypeDisplayName. 36 (ProjectStage) and 535
        // (LeadImplementerOrganization) are read by ImportProjectsAsync.
        dbContext.GisCrossWalkDefaults.AddRange(
            NewCrossWalk(FieldDefinition.TreatmentType.FieldDefinitionID, ThinningSourceValue, TreatmentType.NonCommercial.TreatmentTypeDisplayName),
            NewCrossWalk(FieldDefinition.TreatmentType.FieldDefinitionID, BurnSourceValue, TreatmentType.PrescribedFire.TreatmentTypeDisplayName),
            NewCrossWalk(FieldDefinition.ProjectStage.FieldDefinitionID, ProjectStageSourceValue, ProjectStage.Completed.ProjectStageName),
            NewCrossWalk(FieldDefinition.LeadImplementerOrganization.FieldDefinitionID, LeadImplementerSourceValue, crosswalkedOrganizationName));
        await dbContext.SaveChangesWithNoAuditingAsync();

        var attempt = new GisUploadAttempt
        {
            GisUploadSourceOrganizationID = _sourceOrganizationID,
            GisUploadAttemptCreatePersonID = AssemblySteps.TestAdminPersonID,
            GisUploadAttemptCreateDate = new DateTime(2026, 8, 17, 0, 0, 0, DateTimeKind.Utc)
        };
        dbContext.GisUploadAttempts.Add(attempt);
        await dbContext.SaveChangesWithNoAuditingAsync();
        _attemptID = attempt.GisUploadAttemptID;

        await GisBulkImports.UploadAndProcessFileAsync(dbContext, _attemptID, BuildGeoJson());

        return await BuildRequestAsync(dbContext, _attemptID);
    }

    private GisCrossWalkDefault NewCrossWalk(int fieldDefinitionID, string sourceValue, string mappedValue) => new()
    {
        GisUploadSourceOrganizationID = _sourceOrganizationID,
        FieldDefinitionID = fieldDefinitionID,
        GisCrossWalkSourceValue = sourceValue,
        GisCrossWalkMappedValue = mappedValue
    };

    /// <summary>
    /// One feature per project: a thinning unit, a burn unit, and a unit whose treatment type has no
    /// crosswalk row and so keeps the default treatment type.
    /// </summary>
    private static string BuildGeoJson()
    {
        var featureCollection = new FeatureCollection();
        var factory = new GeometryFactory(new PrecisionModel(), 4326);

        var features = new[]
        {
            (Identifier: ThinningFeatureIdentifier, TreatmentTypeValue: ThinningSourceValue),
            (Identifier: BurnFeatureIdentifier, TreatmentTypeValue: BurnSourceValue),
            (Identifier: UnmappedFeatureIdentifier, TreatmentTypeValue: UnmappedSourceValue),
        };

        for (var i = 0; i < features.Length; i++)
        {
            // Small squares inside Washington so they intersect real County / DNRUplandRegion /
            // PriorityLandscape geometry, matching what a real upload does.
            var minX = -122.0 + i * 0.05;
            var minY = 46.5;
            const double size = 0.02;

            var geometry = factory.CreatePolygon(factory.CreateLinearRing(
            [
                new Coordinate(minX, minY),
                new Coordinate(minX, minY + size),
                new Coordinate(minX + size, minY + size),
                new Coordinate(minX + size, minY),
                new Coordinate(minX, minY)
            ]));

            featureCollection.Add(new Feature(geometry, new AttributesTable
            {
                { "FMA_ID", features[i].Identifier },
                { "FMA_NM", $"PROJECT TYPE DERIVATION UNIT {i}" },
                { "FMA_TYPE_C", features[i].TreatmentTypeValue },
                { "TECHNIQUE_", "PCT" },
                { "ACRES_TREA", Math.Round(12.5 + i, 2) },
                { "PROJ_STAGE", ProjectStageSourceValue },
                { "IMPLEMENTR", LeadImplementerSourceValue },
                { "STAND_ORIG", IsoDate(2020, 4, 28, i) },
                { "FMA_DT", IsoDate(2021, 5, 12, i) }
            }));
        }

        var options = new JsonSerializerOptions();
        options.Converters.Add(new GeoJsonConverterFactory());
        return JsonSerializer.Serialize(featureCollection, options);
    }

    private static string IsoDate(int year, int month, int day, int index) =>
        new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc)
            .AddDays(index)
            .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);

    private static async Task<GisBulkImportRequest> BuildRequestAsync(WADNRDbContext dbContext, int attemptID)
    {
        var attributeIDByName = await dbContext.GisUploadAttemptGisMetadataAttributes
            .AsNoTracking()
            .Where(x => x.GisUploadAttemptID == attemptID)
            .Select(x => new { x.GisMetadataAttributeID, x.GisMetadataAttribute.GisMetadataAttributeName })
            .ToDictionaryAsync(x => x.GisMetadataAttributeName, x => x.GisMetadataAttributeID, StringComparer.OrdinalIgnoreCase);

        int Required(string name) => attributeIDByName.TryGetValue(name, out var id)
            ? id
            : throw new InvalidOperationException(
                $"Expected metadata attribute '{name}' on attempt {attemptID}. Present: {string.Join(", ", attributeIDByName.Keys)}");

        return new GisBulkImportRequest
        {
            ProjectIdentifierMetadataAttributeID = Required("fma_id"),
            ProjectNameMetadataAttributeID = Required("fma_nm"),
            ProjectStageMetadataAttributeID = Required("proj_stage"),
            LeadImplementerMetadataAttributeID = Required("implementr"),
            StartDateMetadataAttributeID = Required("stand_orig"),
            CompletionDateMetadataAttributeID = Required("fma_dt"),
            TreatmentTypeMetadataAttributeID = Required("fma_type_c"),
            TreatmentDetailedActivityTypeMetadataAttributeID = Required("technique_"),
            FootprintAcresMetadataAttributeID = Required("acres_trea"),
            TreatedAcresMetadataAttributeID = Required("acres_trea")
        };
    }

    private async Task<Dictionary<string, string>> ProjectTypeNameByGisIdentifierAsync(WADNRDbContext dbContext)
    {
        dbContext.ChangeTracker.Clear();
        return await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.CreateGisUploadAttemptID == _attemptID && p.ProjectGisIdentifier != null)
            .Select(p => new { p.ProjectGisIdentifier, p.ProjectType.ProjectTypeName })
            .ToDictionaryAsync(x => x.ProjectGisIdentifier!, x => x.ProjectTypeName);
    }

    private async Task<List<int>> ProjectIDsForAttemptAsync(WADNRDbContext dbContext) =>
        await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.CreateGisUploadAttemptID == _attemptID || p.LastUpdateGisUploadAttemptID == _attemptID)
            .Select(p => p.ProjectID)
            .ToListAsync();

    private async Task TearDownFixtureAsync(WADNRDbContext dbContext)
    {
        dbContext.ChangeTracker.Clear();

        var projectIDs = await ProjectIDsForAttemptAsync(dbContext);
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

        var featureIDsQuery = dbContext.GisFeatures
            .Where(f => f.GisUploadAttemptID == _attemptID)
            .Select(f => f.GisFeatureID);

        await dbContext.GisFeatureMetadataAttributes.Where(x => featureIDsQuery.Contains(x.GisFeatureID)).ExecuteDeleteAsync();
        await dbContext.GisFeatures.Where(x => x.GisUploadAttemptID == _attemptID).ExecuteDeleteAsync();
        await dbContext.GisUploadAttemptGisMetadataAttributes.Where(x => x.GisUploadAttemptID == _attemptID).ExecuteDeleteAsync();
        await dbContext.GisUploadAttempts.Where(x => x.GisUploadAttemptID == _attemptID).ExecuteDeleteAsync();

        if (_sourceOrganizationID != 0)
        {
            await dbContext.GisCrossWalkDefaults
                .Where(x => x.GisUploadSourceOrganizationID == _sourceOrganizationID).ExecuteDeleteAsync();
            await dbContext.GisUploadSourceOrganizations
                .Where(x => x.GisUploadSourceOrganizationID == _sourceOrganizationID).ExecuteDeleteAsync();
        }

        if (_programID != 0)
        {
            await dbContext.Programs.Where(x => x.ProgramID == _programID).ExecuteDeleteAsync();
        }

        // GisMetadataAttribute rows are shared global data the production code only ever adds to, so
        // they are deliberately left alone.
        dbContext.ChangeTracker.Clear();
    }

    #endregion
}
