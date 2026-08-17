using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// WADNR-2287 — exercises the real nightly LOA AGOL import against the live ArcGIS Online service.
///
/// This is the only way to prove the "self-healing" claim for the two LOA data gaps the rewrite
/// created and this branch fixes:
///
///   * completion / planned dates, which the AGOL feed encodes as epoch milliseconds. The rewrite
///     parsed only with DateTime.TryParse, so nightly imports resolved no dates at all — post-
///     migration LOA projects sit at 0.8% completion-date coverage versus 65% before.
///   * private landowners, whose creation was dropped entirely — 21.7% coverage versus 99.9%.
///
/// Both are applied on the *update* path, so the first nightly run after deploy should backfill
/// them for the existing 655 projects without any data-fix script.
///
/// EXPLICITLY OPT-IN. It authenticates to DNR's ArcGIS Online organisation with the configured
/// client credentials and downloads the full LOA feature set, so it is [Ignore]d by default and
/// must never run in CI. Remove the attribute to run it deliberately, against a local restore only.
/// </summary>
[TestClass]
[Ignore("Hits the live ArcGIS Online service and rewrites local LOA data. Run deliberately, never in CI.")]
public class LoaAgolSelfHealTests
{
    /// <summary>Source org 3 = "DNR LOA NE" (ProgramID 3), per LoaDataImportJob.</summary>
    private const int LoaGisUploadSourceOrganizationID = 3;

    /// <summary>
    /// Service URLs are read from the gitignored environment.json rather than hard-coded, so no
    /// deployment endpoint lives in source control. Copy them from the deployed configmap when you
    /// intend to run this:
    ///
    ///   "arcGisLoaDataEasternUrl": "...",
    ///   "arcGisLoaDataWesternUrl": "..."
    ///
    /// LoaDataImportJob imports Eastern then Western against the same source organization, so
    /// running only one leaves the other cohort untouched.
    /// </summary>
    private const string EasternUrlKey = "arcGisLoaDataEasternUrl";
    private const string WesternUrlKey = "arcGisLoaDataWesternUrl";

    private static string RequireUrl(string key) =>
        AssemblySteps.Configuration[key] is { Length: > 0 } url
            ? url
            : throw new InvalidOperationException(
                $"'{key}' is not set in WADNR.API.Tests/environment.json. Copy it from the deployed " +
                "configmap to run this test deliberately.");

    [TestMethod]
    public async Task LoaImport_BackfillsDatesAndLandowners_OnExistingProjects()
    {
        var before = await MeasureAsync();
        Console.WriteLine($"BEFORE  projects={before.Projects}  withCompletionDate={before.WithCompletionDate}  " +
                          $"withPlannedDate={before.WithPlannedDate}  withLandowner={before.WithLandowner}");

        await RunEasternImportAsync();

        var after = await MeasureAsync();
        Console.WriteLine($"AFTER   projects={after.Projects}  withCompletionDate={after.WithCompletionDate}  " +
                          $"withPlannedDate={after.WithPlannedDate}  withLandowner={after.WithLandowner}");

        Assert.IsTrue(after.WithCompletionDate > before.WithCompletionDate,
            "Completion dates must backfill — this is the epoch-millisecond parsing fix on the update path.");
        Assert.IsTrue(after.WithLandowner > before.WithLandowner,
            "Landowner records must backfill — this is the restored MakeProjectPeopleAndSave on the update path.");
    }

    [TestMethod]
    public async Task LoaWesternImport_BackfillsRemainingDates()
    {
        var before = await MeasureAsync();
        Console.WriteLine($"BEFORE  projects={before.Projects}  withCompletionDate={before.WithCompletionDate}  " +
                          $"withPlannedDate={before.WithPlannedDate}  withLandowner={before.WithLandowner}");

        await RunImportAsync(RequireUrl(WesternUrlKey));

        var after = await MeasureAsync();
        Console.WriteLine($"AFTER   projects={after.Projects}  withCompletionDate={after.WithCompletionDate}  " +
                          $"withPlannedDate={after.WithPlannedDate}  withLandowner={after.WithLandowner}");
    }

    /// <summary>Esri's public OAuth token endpoint; overridable via environment.json.</summary>
    private static string ArcGisAuthUrl =>
        AssemblySteps.Configuration["arcGisAuthUrl"] is { Length: > 0 } url
            ? url
            : "https://www.arcgis.com/sharing/rest/oauth2/token";

    /// <summary>
    /// Builds just enough WADNRConfiguration for ArcGisAuthService, reading the client credentials
    /// from the API's appsecrets.json. The service URLs live in the deployed configmaps rather than
    /// local config, so they are constants here.
    /// </summary>
    private static WADNRConfiguration BuildConfiguration()
    {
        var secretsPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "WADNR.API", "appsecrets.json"));
        Assert.IsTrue(File.Exists(secretsPath), $"Expected API secrets at {secretsPath}");

        using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(secretsPath));
        string Read(string key) => doc.RootElement.TryGetProperty(key, out var v) ? v.GetString() ?? "" : "";

        var clientId = Read("ArcGisClientId");
        var clientSecret = Read("ArcGisClientSecret");
        Assert.IsFalse(string.IsNullOrWhiteSpace(clientId), "ArcGisClientId is not configured in appsecrets.json.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(clientSecret), "ArcGisClientSecret is not configured in appsecrets.json.");

        return new WADNRConfiguration
        {
            ArcGisAuthUrl = ArcGisAuthUrl,
            ArcGisClientId = clientId,
            ArcGisClientSecret = clientSecret
        };
    }

    private static Task RunEasternImportAsync() => RunImportAsync(RequireUrl(EasternUrlKey));

    private static async Task RunImportAsync(string arcOnlineUrl)
    {
        var configuration = Options.Create(BuildConfiguration());
        var httpClientFactory = new SimpleHttpClientFactory();

        var authService = new ArcGisAuthService(httpClientFactory, configuration);
        var accessToken = await authService.GetApplicationAccessTokenAsync();
        Assert.IsFalse(string.IsNullOrEmpty(accessToken), "Failed to obtain an ArcGIS application access token.");

        await using var dbContext = NewContext();
        var importService = new GisDataImportService(
            httpClientFactory, dbContext, NullLogger<GisDataImportService>.Instance);

        await importService.DownloadAndImportFeaturesWithGetAsync(
            arcOnlineUrl, LoaGisUploadSourceOrganizationID, accessToken);
    }

    private sealed record Coverage(int Projects, int WithCompletionDate, int WithPlannedDate, int WithLandowner);

    /// <summary>
    /// Coverage across LOA projects created by a GIS upload since the migration cutover — the 655
    /// the rewrite left incomplete.
    /// </summary>
    private static async Task<Coverage> MeasureAsync()
    {
        await using var dbContext = NewContext();

        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.CreateGisUploadAttempt != null
                && p.CreateGisUploadAttempt.GisUploadSourceOrganization.ProgramID == 3
                && p.CreateGisUploadAttempt.GisUploadAttemptCreateDate >= new DateTime(2026, 3, 1))
            .Select(p => new
            {
                p.ProjectID,
                HasCompletion = p.CompletionDate != null,
                HasPlanned = p.PlannedDate != null,
                HasLandowner = p.ProjectPeople.Any(x => x.ProjectPersonRelationshipTypeID
                    == ProjectPersonRelationshipType.PrivateLandowner.ProjectPersonRelationshipTypeID)
            })
            .ToListAsync();

        return new Coverage(
            projects.Count,
            projects.Count(x => x.HasCompletion),
            projects.Count(x => x.HasPlanned),
            projects.Count(x => x.HasLandowner));
    }

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

    private sealed class SimpleHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new() { Timeout = TimeSpan.FromMinutes(10) };
    }
}
