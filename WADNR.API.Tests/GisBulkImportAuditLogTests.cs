using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Tests;

/// <summary>
/// Covers WADNR-2288: the GDB bulk import saves without auditing, so projects it created or updated
/// left nothing in the project Audit Log. The import now writes one summary row per project per
/// upload, plus a row for each project field an update actually changed.
///
/// Every test sets ImportAsDetailedLocationInsteadOfTreatments, which skips
/// dbo.procImportTreatmentsFromGisUploadAttempt — the in-memory provider cannot run it.
/// </summary>
[TestClass]
public class GisBulkImportAuditLogTests
{
    private const int ProgramID = 1;
    private const int SourceOrgID = 10;
    private const int AttemptID = 100;
    private const int UploaderPersonID = 42;

    private const int IdentifierAttrID = 1;
    private const int NameAttrID = 2;

    private const int ResearchAndMonitoringProjectTypeID = 1;
    private const int NonCommercialProjectTypeID = 51;
    private const int OtherProjectTypeID = 99;

    private const int DefaultOrganizationID = 1;
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

    /// <summary>Seeds a single-feature upload for identifier "PROJ-1", named "Test Project".</summary>
    private static async Task SeedAsync(WADNRDbContext db, bool adjustProjectType = false)
    {
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = ResearchAndMonitoringProjectTypeID, ProjectTypeName = "Research and Monitoring" });
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = NonCommercialProjectTypeID, ProjectTypeName = "Non-commercial vegetation treatment" });
        db.ProjectTypes.Add(new ProjectType { ProjectTypeID = OtherProjectTypeID, ProjectTypeName = "Other" });

        db.Organizations.Add(new Organization { OrganizationID = DefaultOrganizationID, OrganizationName = "Default Org", OrganizationShortName = "DEF", IsActive = true });

        db.GisUploadSourceOrganizations.Add(new GisUploadSourceOrganization
        {
            GisUploadSourceOrganizationID = SourceOrgID,
            GisUploadSourceOrganizationName = "Test Source",
            ProgramID = ProgramID,
            ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
            AdjustProjectTypeBasedOnTreatmentTypes = adjustProjectType,
            ImportAsDetailedLocationInsteadOfTreatments = true,
            DefaultLeadImplementerOrganizationID = DefaultOrganizationID,
            RelationshipTypeForDefaultOrganizationID = 1,
        });

        db.GisUploadAttempts.Add(new GisUploadAttempt
        {
            GisUploadAttemptID = AttemptID,
            GisUploadSourceOrganizationID = SourceOrgID,
            GisUploadAttemptCreatePersonID = UploaderPersonID,
            GisUploadAttemptCreateDate = new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc),
        });

        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = IdentifierAttrID, GisMetadataAttributeName = "project_id" });
        db.GisMetadataAttributes.Add(new GisMetadataAttribute { GisMetadataAttributeID = NameAttrID, GisMetadataAttributeName = "project_name" });

        db.GisFeatures.Add(new GisFeature
        {
            GisFeatureID = 1000,
            GisUploadAttemptID = AttemptID,
            GisFeatureGeometry = MakeSquare(),
            GisImportFeatureKey = 0,
            IsValid = true,
        });
        db.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute
        {
            GisFeatureMetadataAttributeID = 1, GisFeatureID = 1000, GisMetadataAttributeID = IdentifierAttrID, GisFeatureMetadataAttributeValue = "PROJ-1",
        });
        db.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute
        {
            GisFeatureMetadataAttributeID = 2, GisFeatureID = 1000, GisMetadataAttributeID = NameAttrID, GisFeatureMetadataAttributeValue = "Test Project",
        });

        await db.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Adds a project the import will match on its GIS identifier. Its name differs from the
    /// upload's; its stage and approval status already match what the import will set.
    /// </summary>
    private static async Task SeedExistingProjectAsync(WADNRDbContext db, int projectTypeID, params int[] treatmentTypeIDs)
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
        db.ProjectPrograms.Add(new ProjectProgram { ProjectID = ExistingProjectID, ProgramID = ProgramID });

        var treatmentID = 1;
        foreach (var treatmentTypeID in treatmentTypeIDs)
        {
            db.Treatments.Add(new Treatment
            {
                TreatmentID = treatmentID++,
                ProjectID = ExistingProjectID,
                TreatmentTypeID = treatmentTypeID,
                TreatmentDetailedActivityTypeID = TreatmentDetailedActivityType.Other.TreatmentDetailedActivityTypeID,
                TreatmentFootprintAcres = 10m,
                TreatmentTreatedAcres = 5m,
            });
        }

        await db.SaveChangesWithNoAuditingAsync();
    }

    private static GisBulkImportRequest BuildRequest() => new()
    {
        ProjectIdentifierMetadataAttributeID = IdentifierAttrID,
        ProjectNameMetadataAttributeID = NameAttrID,
    };

    private static Task<List<AuditLog>> AuditLogsForAsync(WADNRDbContext db, int projectID) =>
        db.AuditLogs.AsNoTracking().Where(a => a.ProjectID == projectID).ToListAsync();

    [TestMethod]
    public async Task ImportProjects_WritesCreatedSummaryRow_ForNewProject()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var project = await db.Projects.SingleAsync();
        var log = (await AuditLogsForAsync(db, project.ProjectID)).Single();
        Assert.AreEqual(UploaderPersonID, log.PersonID, "The row must be attributed to the person who ran the upload.");
        Assert.AreEqual(AuditLogEventType.Added.AuditLogEventTypeID, log.AuditLogEventTypeID);
        Assert.AreEqual(GisBulkImports.UploadSummaryAuditLogTableName, log.TableName);
        Assert.AreEqual(AttemptID, log.RecordID);
        Assert.AreEqual($"Project created by GIS bulk upload #{AttemptID} from Test Source", log.AuditDescription);
    }

    [TestMethod]
    public async Task ImportProjects_WritesUpdatedSummaryAndChangedFieldRows_ForExistingProject()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db);
        await SeedExistingProjectAsync(db, OtherProjectTypeID);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var logs = await AuditLogsForAsync(db, ExistingProjectID);
        Assert.IsTrue(logs.All(l => l.PersonID == UploaderPersonID));

        var summary = logs.Single(l => l.TableName == GisBulkImports.UploadSummaryAuditLogTableName);
        Assert.AreEqual(AuditLogEventType.Modified.AuditLogEventTypeID, summary.AuditLogEventTypeID);
        Assert.AreEqual($"Project updated by GIS bulk upload #{AttemptID} from Test Source", summary.AuditDescription);

        var nameChange = logs.Single(l => l.ColumnName == nameof(Project.ProjectName));
        Assert.AreEqual("Existing Project", nameChange.OriginalValue);
        Assert.AreEqual("Test Project", nameChange.NewValue);

        Assert.IsFalse(logs.Any(l => l.ColumnName == nameof(Project.ProjectStageID)),
            "Fields the upload left unchanged must not produce rows.");
        Assert.IsFalse(logs.Any(l => l.ColumnName == nameof(Project.ProjectApprovalStatusID)));
    }

    [TestMethod]
    public async Task ImportProjects_DoesNotWriteBookkeepingColumnRows()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db);
        await SeedExistingProjectAsync(db, OtherProjectTypeID);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var logs = await AuditLogsForAsync(db, ExistingProjectID);
        Assert.IsFalse(logs.Any(l => l.ColumnName == nameof(Project.LastUpdateGisUploadAttemptID)
                                     || l.ColumnName == nameof(Project.CreateGisUploadAttemptID)));
    }

    [TestMethod]
    public async Task ImportProjects_WritesProjectTypeChangeRow_WhenDerivedFromTreatments()
    {
        await using var db = NewInMemoryContext();
        await SeedAsync(db, adjustProjectType: true);
        await SeedExistingProjectAsync(db, ResearchAndMonitoringProjectTypeID, TreatmentType.NonCommercial.TreatmentTypeID);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var typeChange = (await AuditLogsForAsync(db, ExistingProjectID))
            .Single(l => l.ColumnName == nameof(Project.ProjectTypeID));
        Assert.AreEqual(UploaderPersonID, typeChange.PersonID);
        Assert.AreEqual(ResearchAndMonitoringProjectTypeID.ToString(), typeChange.OriginalValue);
        Assert.AreEqual(NonCommercialProjectTypeID.ToString(), typeChange.NewValue);
        Assert.AreEqual("Project Type: Research and Monitoring changed to Non-commercial vegetation treatment", typeChange.AuditDescription);
    }

    [TestMethod]
    public async Task ListForProjectAsGridRow_MapsUploadRowsToGisBulkUploadSection()
    {
        await using var db = NewInMemoryContext();
        db.People.Add(new Person { PersonID = UploaderPersonID, FirstName = "Gis", LastName = "Uploader" });
        await db.SaveChangesWithNoAuditingAsync();
        await SeedAsync(db);

        await GisBulkImports.ImportProjectsAsync(db, AttemptID, BuildRequest());

        var project = await db.Projects.SingleAsync();
        var row = (await AuditLogs.ListForProjectAsGridRowAsync(db, project.ProjectID)).Single();
        Assert.AreEqual("GIS Bulk Upload", row.Section);
        Assert.AreEqual($"Project created by GIS bulk upload #{AttemptID} from Test Source", row.Description);
        Assert.AreEqual("Gis Uploader", row.PersonName);
    }
}
