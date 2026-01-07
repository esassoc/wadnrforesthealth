using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.EFModels.Entities;

namespace WADNR.API.Controllers;

[ApiController]
[Route("file-resources")]
public class FileResourceController(
    WADNRDbContext dbContext,
    ILogger<FileResourceController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration,
    AzureBlobStorageService azureBlobStorageService,
    FileService fileService)
    : SitkaController<FileResourceController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet("{fileResourceGuidAsString}")]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DownloadFileResource(string fileResourceGuidAsString)
    {
        var fileResource = await DbContext.FileResources.AsNoTracking().FirstOrDefaultAsync(x => x.FileResourceGUID.ToString() == fileResourceGuidAsString);

        if (fileResource != null)
        {
            var fileStream = await fileService.GetFileStreamFromBlobStorage(fileResource.FileResourceGUID.ToString());
            if (fileStream != null)
            {
                var fileName = fileResource.OriginalBaseFilename;
                var fileExtension = fileResource.OriginalFileExtension;
                return DisplayFile(fileName, fileExtension, fileStream);
            }
        }

        // Unhappy path - return an HTTP 404
        // ---------------------------------
        var message = $"File resource not found in database. It may have been deleted.";
        logger.LogError(message);
        return NotFound(message);
    }

    private IActionResult DisplayFile(string fileName, string fileExtension, Stream fileStream)
    {
        var contentDisposition = new System.Net.Mime.ContentDisposition
        {
            FileName = $"{fileName}.{fileExtension}",
            Inline = false
        };
        Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(contentDisposition.FileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return File(fileStream, contentType, contentDisposition.FileName);
    }

    [HttpGet("GetWithApiKey/{fileResourceGuidAsString}")]
    public async Task<IActionResult> DisplayResourceWithApiKey([FromRoute] string fileResourceGuidAsString,
        [FromQuery] string apiKey)
    {
        //if (apiKey != _ltInfoConfiguration.LTInfoApiKey)
        //{
        //    return new UnauthorizedResult();
        //}

        //var isStringAGuid = Guid.TryParse(fileResourceGuidAsString, out var fileResourceInfoGuid);
        //if (isStringAGuid)
        //{
        //    var fileResourceInfo = _dbContext.FileResourceInfos.Include(x => x.FileResourceMimeType).Include(x => x.FileResourceData).SingleOrDefault(x => x.FileResourceInfoGUID == fileResourceInfoGuid);

        //    byte[] byteArray = fileResourceInfo.GetFileResourceDatum().Data;
        //    return new FileContentResult(byteArray, "application/octet-stream");
        //}
        //// Unhappy path - return an HTTP 404
        //// ---------------------------------
        //var message = $"File Resource {fileResourceGuidAsString} Not Found in database. It may have been deleted.";
        //_logger.LogError(message);
        //return NotFound(message);

        if (apiKey != Configuration.LTInfoApiKey)
        {
            return new UnauthorizedResult();
        }

        var isStringAGuid = Guid.TryParse(fileResourceGuidAsString, out var fileResourceInfoGuid);
        if (isStringAGuid)
        {
            var fileResource =
                await DbContext.FileResources.FirstOrDefaultAsync(x =>
                    x.FileResourceGUID == fileResourceInfoGuid);

            var fileStream = await fileService.GetFileStreamFromBlobStorage(fileResource.FileResourceGUID.ToString());
            if (fileStream != null)
            {
                var fileName = fileResource.OriginalBaseFilename;
                var fileExtension = fileResource.OriginalFileExtension;
                return DisplayFile(fileName, fileExtension, fileStream);
            }
        }

        // Unhappy path - return an HTTP 404
        // ---------------------------------
        var message = $"File Resource {fileResourceGuidAsString} Not Found in database. It may have been deleted.";
        Logger.LogError(message);
        return NotFound(message);

    }


    //todo: uncomment when we want to sync more fileresources to blob storage
    //[HttpPost("push-to-blob-storage")]
    //public async Task<IActionResult> PushToBlobStorage()
    //{
    //    var nextFileResourcesToMove = DbContext.FileResources
    //        .Where(x => !x.InBlobStorage)
    //        .OrderBy(x => x.FileResourceID)
    //        .ToList();

    //    foreach (var fileResource in nextFileResourcesToMove)
    //    {
    //        Logger.LogInformation($"Begin: Transferring {fileResource.OriginalBaseFilename} to blob storage container.");
    //        var created = azureBlobStorageService.UploadFileResource(fileResource);
    //        Logger.LogInformation($"Finished: Transferring {fileResource.OriginalBaseFilename} to blob storage container.");

    //        fileResource.InBlobStorage = created;
    //    }

    //    await DbContext.SaveChangesAsync();
    //    return Ok();
    //}
}