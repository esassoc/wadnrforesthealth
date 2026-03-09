using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Services;
using WADNR.Common.EMail;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WADNR.API.Hangfire;

public class UsfsDataImportJob(
    ILogger<UsfsDataImportJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient,
    GisDataImportService gisDataImportService)
    : ScheduledBackgroundJobBase<UsfsDataImportJob>(JobName, logger, webHostEnvironment, dbContext,
        configuration, sitkaSmtpClient)
{
    public const string JobName = "USFS Data Import";

    private const int UsfsGisUploadSourceOrganizationID = 14;
    private const int BatchSize = 1000;

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
        // Build WHERE clause from GisCrossWalkDefaults activity codes
        var sourceOrg = await dbContext.GisUploadSourceOrganizations
            .Include(x => x.GisCrossWalkDefaults)
            .SingleAsync(x => x.GisUploadSourceOrganizationID == UsfsGisUploadSourceOrganizationID);

        var activityCodes = sourceOrg.GisCrossWalkDefaults
            .Select(x => $"'{x.GisCrossWalkSourceValue}'")
            .Distinct()
            .ToList();

        var whereClause = $"DATE_COMPLETED>= DATE '2017-01-01' AND ACTIVITY IN ({string.Join(",", activityCodes)})";

        Logger.LogInformation("Starting USFS data import from {Url}", WADNRConfiguration.ArcGisUsfsDataUrl);
        await gisDataImportService.DownloadAndImportFeaturesWithPostAsync(
            WADNRConfiguration.ArcGisUsfsDataUrl,
            UsfsGisUploadSourceOrganizationID,
            whereClause,
            BatchSize,
            useSpatialFilter: true);
    }
}
