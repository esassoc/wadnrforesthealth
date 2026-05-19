using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using WADNR.GDALAPI.Services;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var logger = CreateSerilogLogger(builder);
builder.Host.UseSerilog(logger);

builder.Services.AddControllers();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

builder.Services.AddScoped<Ogr2OgrService>();
builder.Services.AddScoped<OgrInfoService>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromDays(1);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
return;

Logger CreateSerilogLogger(WebApplicationBuilder webApplicationBuilder)
{
    var outputTemplate = $"[{webApplicationBuilder.Environment.EnvironmentName}] {{Timestamp:yyyy-MM-dd HH:mm:ss zzz}} {{Level}} | {{RequestId}}-{{SourceContext}}: {{Message}}{{NewLine}}{{Exception}}";
    var serilogLogger = new LoggerConfiguration()
        .ReadFrom.Configuration(webApplicationBuilder.Configuration)
        .WriteTo.Console(outputTemplate: outputTemplate);
    return serilogLogger.CreateLogger();
}
