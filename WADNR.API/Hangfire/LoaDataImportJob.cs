using System.Collections.Generic;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.Common.EMail;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WADNR.API.Hangfire;

public class LoaDataImportJob(
    ILogger<LoaDataImportJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient,
    ArcGisAuthService arcGisAuthService,
    GisDataImportService gisDataImportService)
    : ScheduledBackgroundJobBase<LoaDataImportJob>(JobName, logger, webHostEnvironment, dbContext,
        configuration, sitkaSmtpClient)
{
    public const string JobName = "LOA Data Import";

    private const int LoaGisUploadSourceOrganizationID = 3;

    public override List<RunEnvironment> RunEnvironments => new()
    {
        RunEnvironment.Production,
        RunEnvironment.Staging,
        //RunEnvironment.Development
    };

    protected override void RunJobImplementation()
    {
        RunImportAsync().GetAwaiter().GetResult();
    }

    private async Task RunImportAsync()
    {
        var accessToken = await arcGisAuthService.GetApplicationAccessTokenAsync();

        // Import Eastern LOA data
        Logger.LogInformation("Starting LOA Eastern data import from {Url}", WADNRConfiguration.ArcGisLoaDataEasternUrl);
        await gisDataImportService.DownloadAndImportFeaturesWithGetAsync(
            WADNRConfiguration.ArcGisLoaDataEasternUrl, LoaGisUploadSourceOrganizationID, accessToken);

        // Import Western LOA data
        Logger.LogInformation("Starting LOA Western data import from {Url}", WADNRConfiguration.ArcGisLoaDataWesternUrl);
        await gisDataImportService.DownloadAndImportFeaturesWithGetAsync(
            WADNRConfiguration.ArcGisLoaDataWesternUrl, LoaGisUploadSourceOrganizationID, accessToken);
    }
}
