using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("firma-home-page-images")]
public class FirmaHomePageImageController(
    WADNRDbContext dbContext,
    ILogger<FirmaHomePageImageController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<FirmaHomePageImageController>(dbContext, logger, configuration)
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".gif", ".png", ".tiff", ".bmp"
    };

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FirmaHomePageImageDetail>>> List()
    {
        var images = await FirmaHomePageImages.ListAsync(DbContext);
        return Ok(images);
    }

    [HttpPost]
    [AdminFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FirmaHomePageImageDetail>> Create(
        [FromForm] string caption,
        [FromForm] int sortOrder,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Image file is required.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
        }

        if (string.IsNullOrWhiteSpace(caption) || caption.Length > 300)
        {
            return BadRequest("Caption is required and must be 300 characters or less.");
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);

        var image = await FirmaHomePageImages.CreateAsync(DbContext, fileResource.FileResourceID, caption.Trim(), sortOrder);

        var images = await FirmaHomePageImages.ListAsync(DbContext);
        return Ok(images);
    }

    [HttpPut("{firmaHomePageImageID}")]
    [AdminFeature]
    public async Task<ActionResult<List<FirmaHomePageImageDetail>>> Update(
        [FromRoute] int firmaHomePageImageID,
        [FromBody] FirmaHomePageImageUpsertRequest request)
    {
        var image = await DbContext.FirmaHomePageImages.FindAsync(firmaHomePageImageID);
        if (image == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Caption) || request.Caption.Length > 300)
        {
            return BadRequest("Caption is required and must be 300 characters or less.");
        }

        await FirmaHomePageImages.UpdateAsync(DbContext, image, request);

        var images = await FirmaHomePageImages.ListAsync(DbContext);
        return Ok(images);
    }

    [HttpDelete("{firmaHomePageImageID}")]
    [AdminFeature]
    public async Task<IActionResult> Delete([FromRoute] int firmaHomePageImageID)
    {
        var image = await DbContext.FirmaHomePageImages
            .Include(x => x.FileResource)
            .FirstOrDefaultAsync(x => x.FirmaHomePageImageID == firmaHomePageImageID);

        if (image == null)
        {
            return NotFound();
        }

        var fileResourceGuid = await FirmaHomePageImages.DeleteAsync(DbContext, image);
        await fileService.DeleteFileStreamFromBlobStorageAsync(fileResourceGuid.ToString());

        return NoContent();
    }

    [HttpPut("sort-order")]
    [AdminFeature]
    public async Task<ActionResult<List<FirmaHomePageImageDetail>>> UpdateSortOrder(
        [FromBody] List<SortOrderUpdateItem> sortOrderUpdates)
    {
        var images = await FirmaHomePageImages.UpdateSortOrderAsync(DbContext, sortOrderUpdates);
        return Ok(images);
    }
}
