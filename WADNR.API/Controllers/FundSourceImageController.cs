using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("fund-source-images")]
public class FundSourceImageController(
    WADNRDbContext dbContext,
    ILogger<FundSourceImageController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService,
    ImageResizeService imageResizeService)
    : SitkaController<FundSourceImageController>(dbContext, logger, configuration)
{
    private const long MaxRawUploadBytes = 30L * 1000 * 1000;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".gif", ".png", ".heic", ".heif"
    };

    [HttpGet("{fundSourceImageID}")]
    [AllowAnonymous]
    public async Task<ActionResult<FundSourceImageDetail>> GetByID([FromRoute] int fundSourceImageID)
    {
        var detail = await FundSourceImages.GetByIDAsDetailAsync(DbContext, fundSourceImageID);
        return RequireNotNullThrowNotFound(detail, "FundSourceImage", fundSourceImageID);
    }

    [HttpPost]
    [FundSourceManageFeature]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxRawUploadBytes)]
    public async Task<ActionResult<FundSourceImageDetail>> Create(
        [FromForm] int fundSourceID,
        [FromForm] string caption,
        [FromForm] string credit,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Image file is required.");
        }

        if (file.Length > MaxRawUploadBytes)
        {
            return BadRequest("Image file is too large. Please choose an image under 30MB.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
        }

        if (string.IsNullOrWhiteSpace(caption) || caption.Length > 200)
        {
            return BadRequest("Caption is required and must be 200 characters or less.");
        }

        if (string.IsNullOrWhiteSpace(credit) || credit.Length > 200)
        {
            return BadRequest("Credit is required and must be 200 characters or less.");
        }

        var fundSourceExists = await DbContext.FundSources.AnyAsync(x => x.FundSourceID == fundSourceID);
        if (!fundSourceExists)
        {
            return NotFound($"Fund source with ID {fundSourceID} not found.");
        }

        // Resize the image to <= 5MB if needed (preserves the original format/extension).
        ResizeResult resizeResult;
        await using (var uploadStream = file.OpenReadStream())
        {
            resizeResult = imageResizeService.ResizeIfNeeded(uploadStream, extension);
        }

        if (!resizeResult.IsValid)
        {
            return BadRequest(resizeResult.ErrorMessage);
        }

        // Create file resource from the (possibly resized/converted) stream.
        var storedFileName = Path.ChangeExtension(file.FileName, resizeResult.Extension);
        FileResource fileResource;
        await using (resizeResult.Stream)
        {
            fileResource = await fileService.CreateFileResource(
                DbContext, resizeResult.Stream, storedFileName, CallingUser.PersonID);
        }

        var fundSourceImage = await FundSourceImages.CreateAsync(
            DbContext,
            fundSourceID,
            fileResource.FileResourceID,
            caption.Trim(),
            credit.Trim());

        var detail = await FundSourceImages.GetByIDAsDetailAsync(DbContext, fundSourceImage.FundSourceImageID);
        return CreatedAtAction(nameof(GetByID), new { fundSourceImageID = fundSourceImage.FundSourceImageID }, detail);
    }

    [HttpPut("{fundSourceImageID}")]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceImageDetail>> Update(
        [FromRoute] int fundSourceImageID,
        [FromBody] FundSourceImageUpsertRequest request)
    {
        var fundSourceImage = await FundSourceImages.GetByIDWithTrackingAsync(DbContext, fundSourceImageID);
        if (fundSourceImage == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Caption) || request.Caption.Length > 200)
        {
            return BadRequest("Caption is required and must be 200 characters or less.");
        }

        if (string.IsNullOrWhiteSpace(request.Credit) || request.Credit.Length > 200)
        {
            return BadRequest("Credit is required and must be 200 characters or less.");
        }

        await FundSourceImages.UpdateAsync(DbContext, fundSourceImage, request);

        var detail = await FundSourceImages.GetByIDAsDetailAsync(DbContext, fundSourceImageID);
        return Ok(detail);
    }

    [HttpDelete("{fundSourceImageID}")]
    [FundSourceManageFeature]
    public async Task<IActionResult> Delete([FromRoute] int fundSourceImageID)
    {
        var fundSourceImage = await FundSourceImages.GetByIDWithFileResourceAsync(DbContext, fundSourceImageID);
        if (fundSourceImage == null)
        {
            return NotFound();
        }

        var fileResourceGuid = await FundSourceImages.DeleteAsync(DbContext, fundSourceImage);

        // Delete from blob storage
        await fileService.DeleteFileStreamFromBlobStorageAsync(fileResourceGuid.ToString());

        return NoContent();
    }

    [HttpPost("{fundSourceImageID}/set-key-photo")]
    [FundSourceManageFeature]
    public async Task<IActionResult> SetKeyPhoto([FromRoute] int fundSourceImageID)
    {
        var fundSourceImage = await FundSourceImages.GetByIDWithTrackingAsync(DbContext, fundSourceImageID);
        if (fundSourceImage == null)
        {
            return NotFound();
        }

        await FundSourceImages.SetKeyPhotoAsync(DbContext, fundSourceImageID);

        return Ok();
    }
}
