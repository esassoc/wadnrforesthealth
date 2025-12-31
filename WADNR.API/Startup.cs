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
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Hangfire;
using Hangfire.SqlServer;
using WADNR.API.Hangfire;
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
            var configuration = Configuration.Get<WADNRConfiguration>();

            services.AddSitkaCaptureService(configuration.SitkaCaptureServiceUrl);

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.Authority = configuration.KeystoneOpenIDUrl;
                    options.RequireHttpsMetadata = false;
                    options.TokenHandlers.Clear();
                    options.TokenHandlers.Add(new JwtSecurityTokenHandler
                    {
                        MapInboundClaims = false
                    });
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";
                });

            services.AddHttpClient("CorralClient")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = _environment.IsDevelopment() ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator : null
                });

            services.AddDbContext<WADNRDbContext>(c =>
            {
                c.UseSqlServer(configuration.DatabaseConnectionString, x =>
                {
                    x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                    x.UseNetTopologySuite();
                });
            });
            services.AddTransient(s => new KeystoneService(s.GetService<IHttpContextAccessor>(), configuration.KeystoneOpenIDUrl));

            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Sendgrid
            // Register SendGrid client from official SDK (not the Extensions DI package)
            services.AddSingleton<ISendGridClient>(_ => new SendGridClient(configuration.SendGridApiKey));
            services.AddSingleton<SitkaSmtpClientService>();
            #endregion

            services.AddScoped(s => s.GetService<IHttpContextAccessor>().HttpContext);
            services.AddScoped(s => UserContext.GetUserFromHttpContext(s.GetService<WADNRDbContext>(), s.GetService<IHttpContextAccessor>().HttpContext));
            services.AddScoped<FileService>();
            services.AddScoped<AzureBlobStorageService>();
            services.AddScoped<IAzureStorage, AzureStorage>();

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

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<EntityNotFoundMiddleware>();
            app.UseMiddleware<LogHelper>();

            #region Hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorizationFilter(Configuration) }
            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            HangfireJobScheduler.ScheduleRecurringJobs();
            #endregion

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
                endpoints.MapHealthChecks("/healthz");
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }
        private void OnShutdown()
        {
            Thread.Sleep(1000);
        }
    }
}
