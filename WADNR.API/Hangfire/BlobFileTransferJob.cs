using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WADNR.API.Services;
using WADNR.Common.EMail;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WADNR.API.Hangfire;

public class BlobFileTransferJob(
    ILogger<BlobFileTransferJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> ltInfoConfiguration,
    SitkaSmtpClientService sitkaSmtpClient,
    AzureBlobStorageService blobStorageService)
    : ScheduledBackgroundJobBase<BlobFileTransferJob>(JobName, logger, webHostEnvironment, dbContext,
        ltInfoConfiguration, sitkaSmtpClient)
{
    private const int FileResourcesPerJob = 1000;

    public override List<RunEnvironment> RunEnvironments => new() { RunEnvironment.Production, RunEnvironment.Staging, RunEnvironment.Development };

    public const string JobName = "Blob File Transfer Job";

    protected override void RunJobImplementation()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var nextFileResourcesToMove = DbContext.FileResources
            .Where(x => !x.InBlobStorage)
            .OrderBy(x => x.FileResourceID)
            .Take(FileResourcesPerJob).ToList();

        foreach (var fileResource in nextFileResourcesToMove)
        {
            Logger.LogInformation($"Begin: Transferring {fileResource.OriginalBaseFilename} to blob storage container.");
            var created = blobStorageService.UploadFileResource(fileResource);
            Logger.LogInformation($"Finished: Transferring {fileResource.OriginalBaseFilename} to blob storage container.");

            fileResource.InBlobStorage = created;
        }

        DbContext.SaveChanges();
        stopwatch.Stop();

        Logger.LogInformation($"Finished transferring {FileResourcesPerJob} FileResources. Job took {stopwatch.Elapsed.TotalSeconds} seconds.");
    }
    

    
}