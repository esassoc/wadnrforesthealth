using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Tests;

/// <summary>
/// Covers WADNR-2150 rework: the LOA / Service Forestry import must carry the source Esri
/// OBJECTID / GlobalID through onto each created ProjectLocation (ArcGisObjectID / ArcGisGlobalID),
/// which the GDB download's ProjectLocations layer then surfaces. Previously these columns were
/// never set and came out blank for every LOA record.
/// </summary>
[TestClass]
public class GisBulkImportArcGisIdTests
{
    private const int ProgramID = 5;
    private const int SourceOrgID = 10;
    private const int AttemptID = 100;

    // Metadata attribute IDs seeded per test.
    private const int IdentifierAttrID = 1;
    private const int NameAttrID = 2;
    private const int ObjectIdAttrID = 3;
    private const int GlobalIdAttrID = 4;

    private static WADNRDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            // ImportProjectsAsync opens a transaction; the in-memory provider ignores transactions
            // and would otherwise raise TransactionIgnoredWarning. Ignore it so the create path runs.
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

    /// <summary>
    /// Seeds the minimum graph for a single new-project create: a source org, an upload attempt,
    /// one ProjectType, and one GisFeature whose metadata always carries an identifier + name and
    /// optionally an objectid / globalid.
    /// </summary>
    private static async Task SeedAsync(WADNRDbContext db, string? objectIdValue, string? globalIdValue)
    {
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = 1, ProjectTypeName = "Default" });

        db.GisUploadSourceOrganizations.Add(new GisUploadSourceOrganization
        {
            GisUploadSourceOrganizationID = SourceOrgID,
            GisUploadSourceOrganizationName = "LOA Test Source",
            ProgramID = ProgramID,
            ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
            ProjectTypeDefaultName = "Default",
            DefaultLeadImplementerOrganizationID = 1,
            RelationshipTypeForDefaultOrganizationID = 1,
            ApplyStartDateToProject = false,
            ApplyCompletedDateToProject = false,
        });

        db.GisUploadAttempts.Add(new GisUploadAttempt
        {
            GisUploadAttemptID = AttemptID,
            GisUploadSourceOrganizationID = SourceOrgID,
            GisUploadAttemptCreatePersonID = 1,
            GisUploadAttemptCreateDate = new DateTime(2026, 6, 16, 0, 0, 0, DateTimeKind.Utc),
        });

        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = IdentifierAttrID, GisMetadataAttributeName = "approval_id" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = NameAttrID, GisMetadataAttributeName = "project_name" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = ObjectIdAttrID, GisMetadataAttributeName = "objectid" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = GlobalIdAttrID, GisMetadataAttributeName = "globalid" });

        db.GisFeatures.Add(new GisFeature
        {
            GisFeatureID = 1000,
            GisUploadAttemptID = AttemptID,
            GisFeatureGeometry = MakeSquare(),
            GisImportFeatureKey = 0,
            IsValid = true,
        });

        var metaID = 1;
        db.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute { GisFeatureMetadataAttributeID = metaID++, GisFeatureID = 1000, GisMetadataAttributeID = IdentifierAttrID, GisFeatureMetadataAttributeValue = "PROJ-1" });
        db.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute { GisFeatureMetadataAttributeID = metaID++, GisFeatureID = 1000, GisMetadataAttributeID = NameAttrID, GisFeatureMetadataAttributeValue = "Test LOA Project" });
        if (objectIdValue != null)
        {
            db.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute { GisFeatureMetadataAttributeID = metaID++, GisFeatureID = 1000, GisMetadataAttributeID = ObjectIdAttrID, GisFeatureMetadataAttributeValue = objectIdValue });
        }
        if (globalIdValue != null)
        {
            db.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute { GisFeatureMetadataAttributeID = metaID++, GisFeatureID = 1000, GisMetadataAttributeID = GlobalIdAttrID, GisFeatureMetadataAttributeValue = globalIdValue });
        }

        await db.SaveChangesWithNoAuditingAsync();
    }

    private static GisBulkImportRequest BuildRequest() => new()
    {
        ProjectIdentifierMetadataAttributeID = IdentifierAttrID,
        ProjectNameMetadataAttributeID = NameAttrID,
    };

    [TestMethod]
    public async Task ImportProjects_SetsArcGisObjectIDAndGlobalID_WhenMetadataPresent()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, objectIdValue: "4242", globalIdValue: "{ABC-123}");

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        var location = await db.ProjectLocations.SingleAsync();
        Assert.AreEqual(4242, location.ArcGisObjectID);
        Assert.AreEqual("{ABC-123}", location.ArcGisGlobalID);
    }

    [TestMethod]
    public async Task ImportProjects_LeavesArcGisColumnsNull_WhenMetadataAbsent()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, objectIdValue: null, globalIdValue: null);

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        var location = await db.ProjectLocations.SingleAsync();
        Assert.IsNull(location.ArcGisObjectID);
        Assert.IsNull(location.ArcGisGlobalID);
    }

    [TestMethod]
    public async Task ImportProjects_DoesNotThrowAndLeavesObjectIDNull_WhenObjectIdUnparseable()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, objectIdValue: "not-a-number", globalIdValue: "{XYZ}");

        var result = await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        Assert.AreEqual(1, result.ProjectsCreated);
        var location = await db.ProjectLocations.SingleAsync();
        Assert.IsNull(location.ArcGisObjectID);
        Assert.AreEqual("{XYZ}", location.ArcGisGlobalID);
    }

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
}
