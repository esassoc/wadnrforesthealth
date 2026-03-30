using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.LoaUpload;

namespace WADNR.API.Controllers;

[ApiController]
[Route("loa-upload")]
public class LoaUploadController(
    WADNRDbContext dbContext,
    ILogger<LoaUploadController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<LoaUploadController>(dbContext, logger, configuration)
{
    [HttpGet("dashboard")]
    [AdminFeature]
    public async Task<ActionResult<LoaUploadDashboard>> GetDashboard()
    {
        var dashboard = await LoaUploads.GetDashboardAsync(DbContext);
        return Ok(dashboard);
    }

    [HttpPost("import/{region}")]
    [AdminFeature]
    [RequestSizeLimit(50_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50_000_000)]
    public async Task<ActionResult<LoaUploadResult>> ImportFile([FromRoute] string region, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { ErrorMessage = "A file is required." });
        }

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { ErrorMessage = "File must be an .xlsx Excel file." });
        }

        bool isNortheast;
        switch (region.ToLowerInvariant())
        {
            case "northeast":
                isNortheast = true;
                break;
            case "southeast":
                isNortheast = false;
                break;
            default:
                return BadRequest(new { ErrorMessage = "Region must be 'northeast' or 'southeast'." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await LoaUploads.ImportLoaFileAsync(DbContext, stream, isNortheast, CallingUser.PersonID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error importing LOA file for region {Region}", region);
            return BadRequest(new { ErrorMessage = $"There was a problem uploading your spreadsheet: {ex.Message}" });
        }
    }

    [HttpPost("publish")]
    [AdminFeature]
    public async Task<ActionResult<LoaPublishingResult>> Publish()
    {
        var result = await LoaUploads.RunPublishingProcessingAsync(DbContext, CallingUser.PersonID);
        if (!result.Success)
        {
            return StatusCode(500, result);
        }
        return Ok(result);
    }
}
