using System.Collections.Generic;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.Common.EMail;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WADNR.API.Hangfire;

public class UsfsNepaBoundaryDataImportJob(
    ILogger<UsfsNepaBoundaryDataImportJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient,
    GisDataImportService gisDataImportService)
    : ScheduledBackgroundJobBase<UsfsNepaBoundaryDataImportJob>(JobName, logger, webHostEnvironment, dbContext,
        configuration, sitkaSmtpClient)
{
    public const string JobName = "USFS NEPA Boundary Data Import";

    private const int UsfsNepaBoundaryGisUploadSourceOrganizationID = 15;
    private const int BatchSize = 100;

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
        Logger.LogInformation("Starting USFS NEPA Boundary data import from {Url}",
            WADNRConfiguration.ArcGisUsfsNepaBoundaryDataUrl);

        await gisDataImportService.DownloadAndImportFeaturesWithPostAsync(
            WADNRConfiguration.ArcGisUsfsNepaBoundaryDataUrl,
            UsfsNepaBoundaryGisUploadSourceOrganizationID,
            whereClause: "1=1",
            BatchSize,
            useSpatialFilter: true);
    }
}
