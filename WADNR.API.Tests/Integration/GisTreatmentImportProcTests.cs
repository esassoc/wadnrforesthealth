using System.Data.Common;
using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Covers how ImportProjectsAsync invokes procImportTreatmentsFromGisUploadAttempt, after that call
/// moved off ExecuteSqlInterpolatedAsync onto the underlying connection with an explicit 600s timeout.
///
/// These tests need a real SQL Server database, and that is the whole point of them. The existing
/// <c>GisBulkImportArcGisIdTests</c> run on the InMemory provider, where
/// <c>Database.GetDbConnection()</c> throws because the provider is not relational — and that throw
/// lands in the same catch that turns proc failures into warnings. So on InMemory the treatment import
/// is silently skipped, and those tests pass whether the proc call works or not.
///
/// That matters more than it sounds, because the failure mode here is invisible. A proc failure is
/// caught and recorded as a warning on an otherwise successful 200 response, so an import that creates
/// zero treatments looks like an import that worked. Switching to
/// <c>CommandType.StoredProcedure</c> made that easier to trigger: parameters are now bound by name,
/// so a typo or a renamed proc parameter produces "Procedure expects parameter which was not
/// supplied" rather than being inert text inside an EXEC string.
/// </summary>
[TestClass]
public class GisTreatmentImportProcTests
{
    /// <summary>Distinct identifier per feature, so each becomes its own project with one treatment.</summary>
    private const int FeatureCount = 3;

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

    #region Tests

    [TestMethod]
    public async Task ImportProjects_CreatesTreatmentsAndReportsNoWarnings_WhenProcRunsAgainstRealSql()
    {
        await using var dbContext = NewContext();
        var request = await ArrangeAsync(dbContext);

        var result = await GisBulkImports.ImportProjectsAsync(dbContext, _attemptID, request);

        // The load-bearing assertion. Everything the proc can go wrong with — a bad parameter name, a
        // type mismatch, a renamed proc — surfaces here and nowhere else, because the catch around the
        // call converts it into a warning instead of a failure.
        Assert.AreEqual(0, result.Warnings.Count,
            "The treatment import proc reported a problem, which an import would otherwise swallow into " +
            $"a successful 200 response: {string.Join(" | ", result.Warnings)}");

        Assert.AreEqual(FeatureCount, result.ProjectsCreated, "Expected one project per distinct identifier.");

        // Warnings being empty only proves nothing threw. Treatments actually existing proves the proc
        // ran and did its work.
        var projectIDs = await ProjectIDsForAttemptAsync(dbContext);
        var treatmentCount = await dbContext.Treatments.CountAsync(t => projectIDs.Contains(t.ProjectID));
        Assert.AreEqual(FeatureCount, treatmentCount,
            "The proc completed without error but created no treatments, so the import silently did nothing.");
    }

    [TestMethod]
    public async Task ImportProjects_RunsProcOutsideEntityFramework_SoTheRetryingExecutionStrategyCannotApply()
    {
        // Why this is worth asserting: EF is configured with EnableRetryOnFailure(maxRetryCount: 3) and
        // SQL timeouts are classed transient, so running the proc through EF meant a slow import was
        // executed up to four times — 4 x CommandTimeout before the caller saw anything, which is what
        // produced the 504. Running it on the raw connection is what removes the execution strategy
        // from the path.
        //
        // This asserts the bypass, not the attempt count. Counting attempts is impossible from outside:
        // the command never reaches EF, so no interceptor can see it, and the timeout is a private
        // const. Proving the command is absent from EF is the strongest observation available without
        // adding a seam to production code for the test's benefit.
        var interceptor = new CommandTextRecordingInterceptor();
        await using var dbContext = NewContext(interceptor);
        var request = await ArrangeAsync(dbContext);

        interceptor.Reset();
        var result = await GisBulkImports.ImportProjectsAsync(dbContext, _attemptID, request);

        Assert.AreEqual(0, result.Warnings.Count,
            $"Import reported warnings, so this test would be measuring a failed proc: {string.Join(" | ", result.Warnings)}");

        var procCommands = interceptor.CommandTexts
            .Where(x => x.Contains("procImportTreatmentsFromGisUploadAttempt", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.AreEqual(0, procCommands.Count,
            "The proc was executed through EF, which puts it back under the retrying execution strategy — " +
            "the exact configuration that turned one slow import into four and caused the 504. " +
            $"Observed: {string.Join(" | ", procCommands)}");

        // Two negative controls, without which the assertion above passes for the wrong reason. The
        // interceptor must be wired up and seeing traffic, and the proc must genuinely have run — an
        // import that skipped the proc entirely would also show zero proc commands.
        Assert.IsTrue(interceptor.CommandTexts.Count > 0,
            "The interceptor recorded no commands at all, so its silence about the proc proves nothing.");

        var projectIDs = await ProjectIDsForAttemptAsync(dbContext);
        var treatmentCount = await dbContext.Treatments.CountAsync(t => projectIDs.Contains(t.ProjectID));
        Assert.AreEqual(FeatureCount, treatmentCount,
            "No treatments were created, so the proc did not run and its absence from EF is meaningless.");
    }

    #endregion

    #region Fixture

    private static WADNRDbContext NewContext(IInterceptor? interceptor = null)
    {
        var connectionString = AssemblySteps.Configuration["sqlConnectionString"]
            ?? throw new InvalidOperationException("sqlConnectionString not found in environment.json");

        // Mirrors production, including EnableRetryOnFailure — the retrying execution strategy is the
        // thing under test, so configuring it away would make the second test vacuous.
        var builder = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseSqlServer(connectionString, x =>
            {
                x.UseNetTopologySuite();
                x.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
            });

        if (interceptor != null)
        {
            builder.AddInterceptors(interceptor);
        }

        return new WADNRDbContext(builder.Options, AssemblySteps.AuditUserProvider);
    }

    /// <summary>
    /// Seeds Program + source org + attempt, then stages features through the real upload path and
    /// returns the import request.
    ///
    /// The features are staged by calling <c>UploadAndProcessFileAsync</c> rather than by hand-inserting
    /// GisFeature rows, because that is what creates the GisMetadataAttribute and
    /// GisUploadAttemptGisMetadataAttribute rows the proc joins through. Hand-seeding those is where a
    /// fixture quietly stops resembling production.
    /// </summary>
    private async Task<GisBulkImportRequest> ArrangeAsync(WADNRDbContext dbContext)
    {
        var program = await ProgramHelper.CreateProgramAsync(
            dbContext, AssemblySteps.TestAdminPersonID, name: $"GIS Treatment Proc Test {DateTime.UtcNow:yyyyMMddHHmmssfff}");
        _programID = program.ProgramID;

        var organizationID = (await dbContext.Organizations.AsNoTracking().OrderBy(x => x.OrganizationID).FirstAsync()).OrganizationID;
        var relationshipTypeID = (await dbContext.RelationshipTypes.AsNoTracking().OrderBy(x => x.RelationshipTypeID).FirstAsync()).RelationshipTypeID;
        var projectTypeName = (await dbContext.ProjectTypes.AsNoTracking().OrderBy(x => x.ProjectTypeID).FirstAsync()).ProjectTypeName;

        var sourceOrganization = new GisUploadSourceOrganization
        {
            GisUploadSourceOrganizationName = $"Treatment Proc Test Source {program.ProgramID}",
            ProgramID = program.ProgramID,
            ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
            ProjectTypeDefaultName = projectTypeName,
            TreatmentTypeDefaultName = null,
            DefaultLeadImplementerOrganizationID = organizationID,
            RelationshipTypeForDefaultOrganizationID = relationshipTypeID,
            AdjustProjectTypeBasedOnTreatmentTypes = false,
            DataDeriveProjectStage = false,
            ImportAsDetailedLocationInsteadOfTreatments = false,
            ImportAsDetailedLocationInAdditionToTreatments = false,
            ApplyStartDateToProject = true,
            ApplyCompletedDateToProject = true,
            ApplyStartDateToTreatments = true,
            ApplyEndDateToTreatments = true,
            ImportIsFlattened = false,
            ProjectDescriptionDefaultText = "Created by GisTreatmentImportProcTests."
        };
        dbContext.GisUploadSourceOrganizations.Add(sourceOrganization);
        await dbContext.SaveChangesWithNoAuditingAsync();
        _sourceOrganizationID = sourceOrganization.GisUploadSourceOrganizationID;

        var attempt = new GisUploadAttempt
        {
            GisUploadSourceOrganizationID = sourceOrganization.GisUploadSourceOrganizationID,
            GisUploadAttemptCreatePersonID = AssemblySteps.TestAdminPersonID,
            GisUploadAttemptCreateDate = new DateTime(2026, 8, 6, 0, 0, 0, DateTimeKind.Utc)
        };
        dbContext.GisUploadAttempts.Add(attempt);
        await dbContext.SaveChangesWithNoAuditingAsync();
        _attemptID = attempt.GisUploadAttemptID;

        await GisBulkImports.UploadAndProcessFileAsync(dbContext, _attemptID, BuildGeoJson());

        return await BuildRequestAsync(dbContext, _attemptID);
    }

    /// <summary>
    /// Field names and value shapes follow the DNR State Lands export, which is what the proc's acreage
    /// and treatment-type branches were written against.
    /// </summary>
    private static string BuildGeoJson()
    {
        var featureCollection = new FeatureCollection();
        var factory = new GeometryFactory(new PrecisionModel(), 4326);

        for (var i = 0; i < FeatureCount; i++)
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
                { "FMA_ID", 9_100_000 + i },
                { "FMA_NM", $"TREATMENT PROC TEST UNIT {i}" },
                { "FMA_TYPE_C", "THIN" },
                { "TECHNIQUE_", "PCT" },
                { "ACRES_TREA", Math.Round(12.5 + i, 2) },
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

    /// <summary>
    /// Builds the request from the metadata attributes the upload stage registered. Names are looked up
    /// lowercased because UploadAndProcessFileAsync stores them via ToLowerInvariant.
    ///
    /// The treatment-related mappings are all supplied deliberately: with them null the proc receives
    /// -1 sentinels, creates nothing, and both tests would pass while proving nothing.
    /// </summary>
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
            StartDateMetadataAttributeID = Required("stand_orig"),
            CompletionDateMetadataAttributeID = Required("fma_dt"),
            TreatmentTypeMetadataAttributeID = Required("fma_type_c"),
            TreatmentDetailedActivityTypeMetadataAttributeID = Required("technique_"),
            FootprintAcresMetadataAttributeID = Required("acres_trea"),
            TreatedAcresMetadataAttributeID = Required("acres_trea")
        };
    }

    private async Task<List<int>> ProjectIDsForAttemptAsync(WADNRDbContext dbContext) =>
        await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.CreateGisUploadAttemptID == _attemptID || p.LastUpdateGisUploadAttemptID == _attemptID)
            .Select(p => p.ProjectID)
            .ToListAsync();

    /// <summary>
    /// Removes everything the fixture and the import created. Children before parents, and treatments
    /// are scoped by ProjectID rather than ProgramID so rows the proc wrote are caught even though it
    /// does not stamp ProgramID.
    /// </summary>
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

    /// <summary>
    /// Records the text of every command EF sends. Used as evidence of absence: the treatment proc must
    /// not appear here, because appearing here would mean it is running under the retrying execution
    /// strategy again.
    /// </summary>
    private sealed class CommandTextRecordingInterceptor : DbCommandInterceptor
    {
        private readonly List<string> _commandTexts = [];
        private readonly object _lock = new();

        public IReadOnlyList<string> CommandTexts
        {
            get { lock (_lock) { return _commandTexts.ToList(); } }
        }

        public void Reset()
        {
            lock (_lock) { _commandTexts.Clear(); }
        }

        // Both the Executing and Failed hooks are recorded, not just the successful ones: a proc that
        // ran through EF and then timed out is precisely the case being guarded against, and it would
        // never reach an "executed" hook.
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            Record(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Record(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            Record(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Record(command);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            Record(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            Record(command);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            Record(command);
            base.CommandFailed(command, eventData);
        }

        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            Record(command);
            return base.CommandFailedAsync(command, eventData, cancellationToken);
        }

        private void Record(DbCommand command)
        {
            lock (_lock) { _commandTexts.Add(command.CommandText ?? ""); }
        }
    }
}
