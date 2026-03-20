using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using NetTopologySuite.IO.Converters;
using Scalar.AspNetCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using WADNR.EFModels.Entities;
using WADNR.Scalar;
using WADNR.Scalar.Filters;
using WADNR.Scalar.Logging;
using WADNR.Scalar.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddEnvironmentVariables()
    .AddJsonFile(builder.Configuration["SECRET_PATH"] ?? "appsecrets.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: true);

// Logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration);
});

// Services
builder.Services.Configure<WADNRScalarConfiguration>(builder.Configuration);
var configuration = builder.Configuration.Get<WADNRScalarConfiguration>();

builder.Services.AddDbContext<WADNRDbContext>(c =>
{
    c.UseSqlServer(configuration.DatabaseConnectionString, x =>
    {
        x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
        x.UseNetTopologySuite();
    });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory(false));
    options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Share GeoJSON converter with the OpenAPI schema generator so it can describe NTS geometry types
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new GeoJsonConverterFactory(false));
    options.SerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddOpenApi(options =>
{
    options.AddScalarTransformers();

    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        if (typeof(Geometry).IsAssignableFrom(context.JsonTypeInfo.Type))
        {
            schema.Type = JsonSchemaType.Object;
            schema.Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "GeoJSON geometry type (e.g. Point, Polygon)" },
                ["coordinates"] = new OpenApiSchema { Type = JsonSchemaType.Array, Description = "GeoJSON coordinates array" }
            };
            schema.Description = "GeoJSON geometry object";
        }
        return Task.CompletedTask;
    });

    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "WA DNR Forest Health Tracker API",
            Version = "1.0",
            Description =
                "The WA DNR Forest Health Tracker REST API provides resource-oriented URLs to fetch data as JSON. " +
                "To use this API, you will need an API key. Log in to the Forest Health Tracker, navigate to your profile, " +
                "and generate an API key. Pass the key in the `x-api-key` header with every request.",
        };

        // Use relative server URL so Scalar test requests inherit the current page's scheme/host (HTTPS)
        document.Servers = [new OpenApiServer { Url = "/" }];

        return Task.CompletedTask;
    });

    options.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>();
});

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "ApiKeyScheme";
    options.DefaultChallengeScheme = "ApiKeyScheme";
})
.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", _ => { });

builder.Services.AddHealthChecks().AddDbContextCheck<WADNRDbContext>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Middleware
app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
    opts.GetLevel = LogHelper.CustomGetLevel;
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.WithExposedHeaders("WWW-Authenticate");
});
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AccessDeniedMiddleware>();
app.UseMiddleware<LogHelper>();
app.MapControllers();
app.MapHealthChecks("/healthz");

app.MapOpenApi();
app.MapScalarApiReference("/docs", options =>
{
    options.Title = "WA DNR Forest Health Tracker API";
    options.ShowSidebar = true;
    options.HideModels = true;
    options.AddPreferredSecuritySchemes("ApiKeyScheme");
    options.DefaultHttpClient =
        new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);

    // Provide placeholder so code snippets include the x-api-key header
    options.AddApiKeyAuthentication("ApiKeyScheme", scheme =>
        scheme.WithName("x-api-key").WithValue("YOUR_API_KEY_HERE"));
});

app.Run();

internal sealed class ApiKeySecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.All(authScheme => authScheme.Name == "ApiKeyScheme"))
        {
            var requirements = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["ApiKeyScheme"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme",
                    In = ParameterLocation.Header,
                    Name = "x-api-key"
                }
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("ApiKeyScheme")] = new List<string>()
                });
            }
        }
    }
}
