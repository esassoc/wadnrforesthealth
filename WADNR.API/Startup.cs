using WADNR.API.Logging;
using WADNR.API.Services;
using WADNR.API.Services.Filter;
using WADNR.Common.EMail;
using WADNR.Common.JsonConverters;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetTopologySuite.IO.Converters;
using SendGrid;
using Serilog;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Hangfire.SqlServer;
using WADNR.API.Hangfire;
using WADNR.API.Services.Authentication;
using WADNR.API.Services.Middleware;
using ILogger = Serilog.ILogger;

namespace WADNR.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                })
                .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory(false));
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new DoubleConverter(7));
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            });

            services.Configure<WADNRConfiguration>(Configuration);
            services.Configure<SendGridConfiguration>(Configuration);
            var configuration = Configuration.Get<WADNRConfiguration>();

            services.AddSitkaCaptureService(configuration.SitkaCaptureServiceUrl);

            var enableTestAuth = configuration.EnableE2ETestAuth;
            if (enableTestAuth)
            {
                // Register both JWT and E2E test auth; a policy scheme picks which to use per-request
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "DualAuth";
                    options.DefaultChallengeScheme = "DualAuth";
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://wadnr.us.auth0.com/";
                    options.Audience = "WADNRAPI";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, null)
                .AddPolicyScheme("DualAuth", "JWT or E2E Test Auth", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        if (context.Request.Headers.ContainsKey(TestAuthHandler.TestUserHeader))
                            return TestAuthHandler.SchemeName;
                        return JwtBearerDefaults.AuthenticationScheme;
                    };
                });
                Log.Warning("E2E Test authentication is ENABLED - do not use in production!");
            }
            else
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.Authority = "https://wadnr.us.auth0.com/";
                    options.Audience = "WADNRAPI";
                });
            }

            // Require authentication by default - endpoints must explicitly use [AllowAnonymous] for public access
            services.AddAuthorizationBuilder()
                .SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build());

            services.AddDbContext<WADNRDbContext>(c =>
            {
                c.UseSqlServer(configuration.DatabaseConnectionString, x =>
                {
                    x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                    x.UseNetTopologySuite();
                });
            });

            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Sendgrid
            // Register SendGrid client from official SDK (not the Extensions DI package)
            services.AddSingleton<ISendGridClient>(_ => new SendGridClient(configuration.SendGridApiKey));
            services.AddSingleton<SitkaSmtpClientService>();
            #endregion

            services.AddScoped(s => s.GetService<IHttpContextAccessor>().HttpContext);
            services.AddScoped(s => UserContext.GetUserAsDetailFromHttpContext(s.GetService<WADNRDbContext>(), s.GetService<IHttpContextAccessor>().HttpContext));
            services.AddScoped<FileService>();
            services.AddScoped<AzureBlobStorageService>();
            services.AddScoped<IAzureStorage, AzureStorage>();
            services.AddScoped<ProjectNotificationService>();
            services.AddScoped<IAuditUserProvider, HttpContextAuditUserProvider>();

            #region Hangfire Job Services
            services.AddHttpClient("FinanceApi", c =>
            {
                c.Timeout = TimeSpan.FromMinutes(10);
            });
            services.AddHttpClient("GisApi", c =>
            {
                c.Timeout = TimeSpan.FromMinutes(30);
            });
            services.AddScoped<ArcGisAuthService>();
            services.AddScoped<FinanceApiDownloadService>();
            services.AddScoped<GisDataImportService>();
            #endregion

            #region GDAL API
            if (!string.IsNullOrEmpty(configuration.GDALAPIBaseUrl))
            {
                services.AddHttpClient<GDALAPIService>(c =>
                {
                    c.BaseAddress = new Uri(configuration.GDALAPIBaseUrl);
                    c.Timeout = TimeSpan.FromMinutes(30);
                }).ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return handler;
                });
            }
            #endregion

            #region Hangfire
            services.AddHangfire(c => c
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.DatabaseConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    PrepareSchemaIfNecessary = false,
                }));

            services.AddHangfireServer(x =>
            {
                x.WorkerCount = 1;
            });
            #endregion

            #region Swagger
            // Base swagger services
            services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<UseMethodNameAsOperationIdFilter>();
                options.OperationFilter<AnonymousOperationFilter>();
            });
            #endregion

            services.AddHealthChecks().AddDbContextCheck<WADNRDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory, ILogger logger)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.GetLevel = LogHelper.CustomGetLevel;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(policy =>
            {
                //TODO: don't allow all origins
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.WithExposedHeaders("WWW-Authenticate");
            });

            #region Hangfire
            // Hangfire dashboard must be registered BEFORE UseAuthentication/UseAuthorization
            // so the fallback JWT auth policy doesn't intercept /hangfire requests and override
            // the Basic auth WWW-Authenticate header that triggers the browser login prompt.
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorizationFilter(Configuration) }
            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            HangfireJobScheduler.ScheduleRecurringJobs();
            #endregion

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<EntityNotFoundMiddleware>();
            app.UseMiddleware<LogHelper>();

            #region Swagger
            // Register swagger middleware and enable the swagger UI which will be 
            // accessible at https://<apihostname>/swagger
            // NOTE: There is no auth on these endpoints out of the box.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "V1");
            });
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz").AllowAnonymous();
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }
        private void OnShutdown()
        {
            Thread.Sleep(1000);
        }
    }
}
