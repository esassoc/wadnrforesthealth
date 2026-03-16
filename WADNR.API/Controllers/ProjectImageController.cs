using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
[Route("project-images")]
public class ProjectImageController(
    WADNRDbContext dbContext,
    ILogger<ProjectImageController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<ProjectImageController>(dbContext, logger, configuration)
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".gif", ".png"
    };

    [HttpGet("timings")]
    [AllowAnonymous]
    public ActionResult<List<ProjectImageTimingLookupItem>> ListTimings()
    {
        var timings = ProjectImages.ListTimingAsLookupItem();
        return Ok(timings);
    }

    [HttpGet("{projectImageID}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProjectImageDetail>> GetByID([FromRoute] int projectImageID)
    {
        var detail = await ProjectImages.GetByIDAsDetailAsync(DbContext, projectImageID);
        if (detail == null)
        {
            return NotFound();
        }
        return Ok(detail);
    }

    [HttpPost]
    [ProjectEditAsAdminFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProjectImageDetail>> Create(
        [FromForm] int projectID,
        [FromForm] string caption,
        [FromForm] string credit,
        [FromForm] int? projectImageTimingID,
        [FromForm] bool excludeFromFactSheet,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Image file is required.");
        }

        // Validate file extension
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
        }

        // Validate caption length
        if (string.IsNullOrWhiteSpace(caption) || caption.Length > 200)
        {
            return BadRequest("Caption is required and must be 200 characters or less.");
        }

        // Validate credit length
        if (string.IsNullOrWhiteSpace(credit) || credit.Length > 200)
        {
            return BadRequest("Credit is required and must be 200 characters or less.");
        }

        // Verify project exists
        var project = await Projects.GetByIDWithTrackingAsync(DbContext, projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        // Create file resource
        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);

        // Create project image
        var projectImage = await ProjectImages.CreateAsync(
            DbContext,
            projectID,
            fileResource.FileResourceID,
            caption.Trim(),
            credit.Trim(),
            projectImageTimingID,
            excludeFromFactSheet);

        var detail = await ProjectImages.GetByIDAsDetailAsync(DbContext, projectImage.ProjectImageID);
        return CreatedAtAction(nameof(GetByID), new { projectImageID = projectImage.ProjectImageID }, detail);
    }

    [HttpPut("{projectImageID}")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<ProjectImageDetail>> Update(
        [FromRoute] int projectImageID,
        [FromBody] ProjectImageUpsertRequest request)
    {
        var projectImage = await ProjectImages.GetByIDWithTrackingAsync(DbContext, projectImageID);
        if (projectImage == null)
        {
            return NotFound();
        }

        // Validate caption length
        if (string.IsNullOrWhiteSpace(request.Caption) || request.Caption.Length > 200)
        {
            return BadRequest("Caption is required and must be 200 characters or less.");
        }

        // Validate credit length
        if (string.IsNullOrWhiteSpace(request.Credit) || request.Credit.Length > 200)
        {
            return BadRequest("Credit is required and must be 200 characters or less.");
        }

        await ProjectImages.UpdateAsync(DbContext, projectImage, request);

        var detail = await ProjectImages.GetByIDAsDetailAsync(DbContext, projectImageID);
        return Ok(detail);
    }

    [HttpDelete("{projectImageID}")]
    [ProjectEditAsAdminFeature]
    public async Task<IActionResult> Delete([FromRoute] int projectImageID)
    {
        var projectImage = await ProjectImages.GetByIDWithFileResourceAsync(DbContext, projectImageID);
        if (projectImage == null)
        {
            return NotFound();
        }

        var fileResourceGuid = await ProjectImages.DeleteAsync(DbContext, projectImage);

        // Delete from blob storage
        await fileService.DeleteFileStreamFromBlobStorageAsync(fileResourceGuid.ToString());

        return NoContent();
    }

    [HttpPost("{projectImageID}/set-key-photo")]
    [ProjectEditAsAdminFeature]
    public async Task<IActionResult> SetKeyPhoto([FromRoute] int projectImageID)
    {
        var projectImage = await ProjectImages.GetByIDWithTrackingAsync(DbContext, projectImageID);
        if (projectImage == null)
        {
            return NotFound();
        }

        await ProjectImages.SetKeyPhotoAsync(DbContext, projectImageID);

        return Ok();
    }
}
