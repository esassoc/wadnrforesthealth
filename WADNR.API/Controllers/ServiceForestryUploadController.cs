using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.ServiceForestryUpload;

namespace WADNR.API.Controllers;

[ApiController]
[Route("service-forestry-upload")]
public class ServiceForestryUploadController(
    WADNRDbContext dbContext,
    ILogger<ServiceForestryUploadController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ServiceForestryUploadController>(dbContext, logger, configuration)
{
    [HttpGet("dashboard")]
    [AdminFeature]
    public async Task<ActionResult<ServiceForestryUploadDashboard>> GetDashboard()
    {
        var dashboard = await ServiceForestryUploads.GetDashboardAsync(DbContext);
        return Ok(dashboard);
    }

    [HttpPost("import")]
    [AdminFeature]
    [RequestSizeLimit(50_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50_000_000)]
    public async Task<ActionResult<ServiceForestryUploadResult>> ImportFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { ErrorMessage = "A file is required." });
        }

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { ErrorMessage = "File must be an .xlsx Excel file." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await ServiceForestryUploads.ImportServiceForestryFileAsync(DbContext, stream, CallingUser.PersonID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error importing Service Forestry file");
            return BadRequest(new { ErrorMessage = $"There was a problem uploading your spreadsheet: {ex.Message}" });
        }
    }

    [HttpPost("publish")]
    [AdminFeature]
    public async Task<ActionResult<ServiceForestryPublishingResult>> Publish()
    {
        var result = await ServiceForestryUploads.RunPublishingProcessingAsync(DbContext, CallingUser.PersonID);
        if (!result.Success)
        {
            return StatusCode(500, result);
        }
        return Ok(result);
    }
}
