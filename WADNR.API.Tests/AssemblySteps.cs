using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.IO.Converters;
using WADNR.API.Tests.Helpers;
using WADNR.API.Services.Authentication;
using WADNR.Common.EMail;
using WADNR.Common.JsonConverters;
using WADNR.EFModels.Entities;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

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
    /// GlobalID for the test admin person (used for HTTP auth).
    /// </summary>
    public static string TestAdminGlobalID { get; private set; } = null!;

    /// <summary>
    /// GlobalID for the test normal person (used for HTTP auth).
    /// </summary>
    public static string TestNormalGlobalID { get; private set; } = null!;

    /// <summary>
    /// HttpClient authenticated as admin user.
    /// </summary>
    public static HttpClient AdminHttpClient { get; private set; } = null!;

    /// <summary>
    /// HttpClient authenticated as normal user.
    /// </summary>
    public static HttpClient NormalHttpClient { get; private set; } = null!;

    /// <summary>
    /// HttpClient with no authentication.
    /// </summary>
    public static HttpClient UnauthenticatedHttpClient { get; private set; } = null!;

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
        await SetupAPI();
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

            // Resolve GlobalIDs for the test persons (needed for HTTP auth via TestAuthHandler).
            // If the configured persons don't have GlobalIDs, find persons that do (preferring Admin/Normal roles).
            // If no persons have GlobalIDs at all, assign synthetic ones to the test persons.
            var adminGlobalID = await tempContext.People.AsNoTracking()
                .Where(p => p.PersonID == TestAdminPersonID)
                .Select(p => p.GlobalID)
                .SingleAsync();

            var normalGlobalID = await tempContext.People.AsNoTracking()
                .Where(p => p.PersonID == TestNormalPersonID)
                .Select(p => p.GlobalID)
                .SingleAsync();

            if (adminGlobalID == null)
            {
                // Try to find any person with Admin role and a GlobalID
                var altAdmin = await tempContext.People.AsNoTracking()
                    .Where(p => p.GlobalID != null && p.PersonRoles.Any(pr => pr.RoleID == (int)RoleEnum.Admin || pr.RoleID == (int)RoleEnum.EsaAdmin))
                    .Select(p => new { p.PersonID, p.GlobalID })
                    .FirstOrDefaultAsync();

                if (altAdmin != null)
                {
                    TestAdminPersonID = altAdmin.PersonID;
                    adminGlobalID = altAdmin.GlobalID;
                    Console.WriteLine($"Configured admin person had no GlobalID. Using PersonID={altAdmin.PersonID} (has Admin role + GlobalID).");
                }
                else
                {
                    // Last resort: assign a synthetic GlobalID and ensure Admin role
                    var adminEntity = await tempContext.People
                        .Include(p => p.PersonRoles)
                        .SingleAsync(p => p.PersonID == TestAdminPersonID);
                    adminEntity.GlobalID = $"test-admin-{Guid.NewGuid()}";
                    if (!adminEntity.PersonRoles.Any(pr => pr.RoleID == (int)RoleEnum.Admin))
                    {
                        tempContext.PersonRoles.Add(new PersonRole { PersonID = TestAdminPersonID, RoleID = (int)RoleEnum.Admin });
                    }
                    await tempContext.SaveChangesAsync();
                    adminGlobalID = adminEntity.GlobalID;
                    Console.WriteLine($"No persons with Admin role + GlobalID found. Assigned synthetic GlobalID + Admin role to PersonID={TestAdminPersonID}.");
                }
            }

            if (normalGlobalID == null)
            {
                // Try to find any person with Normal role and a GlobalID
                var altNormal = await tempContext.People.AsNoTracking()
                    .Where(p => p.GlobalID != null && p.PersonRoles.Any(pr => pr.RoleID == (int)RoleEnum.Normal))
                    .Select(p => new { p.PersonID, p.GlobalID })
                    .FirstOrDefaultAsync();

                if (altNormal != null)
                {
                    TestNormalPersonID = altNormal.PersonID;
                    normalGlobalID = altNormal.GlobalID;
                    Console.WriteLine($"Configured normal person had no GlobalID. Using PersonID={altNormal.PersonID} (has Normal role + GlobalID).");
                }
                else
                {
                    // Last resort: assign a synthetic GlobalID and ensure Normal role
                    var normalEntity = await tempContext.People
                        .Include(p => p.PersonRoles)
                        .SingleAsync(p => p.PersonID == TestNormalPersonID);
                    normalEntity.GlobalID = $"test-normal-{Guid.NewGuid()}";
                    if (!normalEntity.PersonRoles.Any(pr => pr.RoleID == (int)RoleEnum.Normal))
                    {
                        tempContext.PersonRoles.Add(new PersonRole { PersonID = TestNormalPersonID, RoleID = (int)RoleEnum.Normal });
                    }
                    await tempContext.SaveChangesAsync();
                    normalGlobalID = normalEntity.GlobalID;
                    Console.WriteLine($"No persons with Normal role + GlobalID found. Assigned synthetic GlobalID + Normal role to PersonID={TestNormalPersonID}.");
                }
            }

            TestAdminGlobalID = adminGlobalID!;
            TestNormalGlobalID = normalGlobalID!;

            // Ensure the resolved persons have the required roles for HTTP auth tests
            var adminHasAdminRole = await tempContext.PersonRoles.AnyAsync(pr =>
                pr.PersonID == TestAdminPersonID && (pr.RoleID == (int)RoleEnum.Admin || pr.RoleID == (int)RoleEnum.EsaAdmin));
            if (!adminHasAdminRole)
            {
                tempContext.PersonRoles.Add(new PersonRole { PersonID = TestAdminPersonID, RoleID = (int)RoleEnum.Admin });
                await tempContext.SaveChangesAsync();
                Console.WriteLine($"Added Admin role to test admin PersonID={TestAdminPersonID}.");
            }

            var normalHasNormalRole = await tempContext.PersonRoles.AnyAsync(pr =>
                pr.PersonID == TestNormalPersonID && pr.RoleID == (int)RoleEnum.Normal);
            if (!normalHasNormalRole)
            {
                tempContext.PersonRoles.Add(new PersonRole { PersonID = TestNormalPersonID, RoleID = (int)RoleEnum.Normal });
                await tempContext.SaveChangesAsync();
                Console.WriteLine($"Added Normal role to test normal PersonID={TestNormalPersonID}.");
            }
        }

        // Create the audit user provider with the admin user by default
        AuditUserProvider = new TestAuditUserProvider(TestAdminPersonID);

        // Create the main DbContext with the audit user provider
        DbContext = new WADNRDbContext(dbOptions, AuditUserProvider);
    }

    private static async Task SetupAPI()
    {
        var connectionString = Configuration["sqlConnectionString"]!;

        var webApplicationFactory = new WebApplicationFactory<WADNR.API.Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                var inMemorySettings = new Dictionary<string, string?>
                {
                    ["DatabaseConnectionString"] = connectionString,
                    ["EnableE2ETestAuth"] = "true",
                    ["Auth0:Authority"] = "https://test.auth0.com/",
                    ["Auth0:Audience"] = "test-audience",
                    ["SendGridApiKey"] = "fake-key",
                    ["SitkaCaptureServiceUrl"] = "https://localhost:9999",
                    ["GDALAPIBaseUrl"] = "",
                };

                conf.AddInMemoryCollection(inMemorySettings);
            });

            builder.ConfigureTestServices(services =>
            {
                // Remove existing DbContext and re-add with test connection string
                var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<WADNRDbContext>));
                if (dbDescriptor != null)
                {
                    services.Remove(dbDescriptor);
                }

                services.AddDbContext<WADNRDbContext>(c =>
                {
                    c.UseSqlServer(connectionString, x =>
                    {
                        x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                        x.UseNetTopologySuite();
                    });
                });

                // Remove existing Hangfire registrations and re-add minimal config
                var hangfireDescriptors = services
                    .Where(d => d.ServiceType.Assembly.FullName?.Contains("Hangfire") == true)
                    .ToList();

                foreach (var descriptor in hangfireDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseLogProvider(new NullLogProvider())
                    .UseSqlServerStorage(connectionString, new global::Hangfire.SqlServer.SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

                services.AddHangfireServer(options => { options.WorkerCount = 1; });

                // Replace email service with fake
                var smtpDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(SitkaSmtpClientService));
                if (smtpDescriptor != null)
                    services.Remove(smtpDescriptor);
                services.AddSingleton<SitkaSmtpClientService, FakeSitkaSmtpClientService>();
            });
        });

        // Create pre-authenticated HttpClients using the existing TestAuthHandler
        // TestAuthHandler reads X-E2E-User-GlobalID header and sets Sub claim
        AdminHttpClient = webApplicationFactory.CreateClient();
        AdminHttpClient.DefaultRequestHeaders.Add(TestAuthHandler.TestUserHeader, TestAdminGlobalID);
        AdminHttpClient.Timeout = TimeSpan.FromMinutes(3);

        NormalHttpClient = webApplicationFactory.CreateClient();
        NormalHttpClient.DefaultRequestHeaders.Add(TestAuthHandler.TestUserHeader, TestNormalGlobalID);
        NormalHttpClient.Timeout = TimeSpan.FromMinutes(3);

        UnauthenticatedHttpClient = webApplicationFactory.CreateClient();
        UnauthenticatedHttpClient.Timeout = TimeSpan.FromMinutes(3);

        // Warm up with a first request (always slower)
        await AdminHttpClient.GetAsync("healthz");
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

    internal class NullLogProvider : global::Hangfire.Logging.ILogProvider
    {
        public global::Hangfire.Logging.ILog GetLogger(string name) => new NullLogger();

        private class NullLogger : global::Hangfire.Logging.ILog
        {
            public bool Log(global::Hangfire.Logging.LogLevel logLevel, Func<string> messageFunc, Exception? exception = null)
            {
                return false;
            }
        }
    }
}
