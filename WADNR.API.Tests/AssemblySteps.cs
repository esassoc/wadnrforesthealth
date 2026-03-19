using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.IO.Converters;
using WADNR.API.Tests.Helpers;
using WADNR.Common.JsonConverters;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests;

[TestClass]
public static class AssemblySteps
{
    public static IConfigurationRoot Configuration => new ConfigurationBuilder()
        .AddJsonFile(@"environment.json", optional: false)
        .Build();

    /// <summary>
    /// Shared DbContext for integration tests. Uses a real SQL Server database.
    /// </summary>
    public static WADNRDbContext DbContext { get; private set; } = null!;

    /// <summary>
    /// Audit user provider that can be set per test.
    /// </summary>
    public static TestAuditUserProvider AuditUserProvider { get; private set; } = null!;

    /// <summary>
    /// Test admin person ID from configuration.
    /// </summary>
    public static int TestAdminPersonID { get; private set; }

    /// <summary>
    /// Test normal user person ID from configuration.
    /// </summary>
    public static int TestNormalPersonID { get; private set; }

    /// <summary>
    /// Default JSON serializer options matching the API configuration.
    /// </summary>
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new GeoJsonConverterFactory(false), new DoubleConverter(7), new JsonStringEnumConverter() }
    };

    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext testContext)
    {
        await SetupDatabase();
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        DbContext?.Dispose();
    }

    private static async Task SetupDatabase()
    {
        var connectionString = Configuration["sqlConnectionString"]
            ?? throw new InvalidOperationException("sqlConnectionString not found in environment.json");

        var configAdminID = int.Parse(Configuration["testAdminPersonID"] ?? "1");
        var configNormalID = int.Parse(Configuration["testNormalPersonID"] ?? "2");

        // Create DbContext options
        var dbOptions = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseSqlServer(connectionString, x => x.UseNetTopologySuite())
            .Options;

        // Create a temporary context without audit provider to query persons
        using (var tempContext = new WADNRDbContext(dbOptions))
        {
            // Verify the configured person IDs exist or get valid ones
            var adminExists = await tempContext.People.AnyAsync(p => p.PersonID == configAdminID);
            var normalExists = await tempContext.People.AnyAsync(p => p.PersonID == configNormalID);

            if (!adminExists || !normalExists)
            {
                // Get the first two persons from the database
                var persons = await tempContext.People.Take(2).ToListAsync();
                if (persons.Count == 0)
                {
                    throw new InvalidOperationException("No persons found in the database. Tests require at least one person.");
                }

                TestAdminPersonID = persons[0].PersonID;
                TestNormalPersonID = persons.Count > 1 ? persons[1].PersonID : persons[0].PersonID;

                Console.WriteLine($"Configured person IDs not found. Using PersonIDs from database: Admin={TestAdminPersonID}, Normal={TestNormalPersonID}");
            }
            else
            {
                TestAdminPersonID = configAdminID;
                TestNormalPersonID = configNormalID;
            }
        }

        // Create the audit user provider with the admin user by default
        AuditUserProvider = new TestAuditUserProvider(TestAdminPersonID);

        // Create the main DbContext with the audit user provider
        DbContext = new WADNRDbContext(dbOptions, AuditUserProvider);
    }

    /// <summary>
    /// Creates a fresh DbContext instance for tests that need isolated change tracking.
    /// </summary>
    public static WADNRDbContext CreateFreshDbContext()
    {
        var connectionString = Configuration["sqlConnectionString"]
            ?? throw new InvalidOperationException("sqlConnectionString not found in environment.json");

        var dbOptions = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseSqlServer(connectionString, x => x.UseNetTopologySuite())
            .Options;

        return new WADNRDbContext(dbOptions, AuditUserProvider);
    }

    /// <summary>
    /// Sets the current user for audit logging.
    /// </summary>
    public static void SetCurrentUser(int personID)
    {
        AuditUserProvider.SetPersonID(personID);
    }
}
