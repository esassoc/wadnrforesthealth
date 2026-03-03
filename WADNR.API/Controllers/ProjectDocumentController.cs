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
[Route("project-documents")]
public class ProjectDocumentController(
    WADNRDbContext dbContext,
    ILogger<ProjectDocumentController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<ProjectDocumentController>(dbContext, logger, configuration)
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".zip", ".doc", ".docx", ".xls", ".xlsx"
    };

    [HttpGet("{projectDocumentID}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProjectDocumentDetail>> GetByID([FromRoute] int projectDocumentID)
    {
        var detail = await ProjectDocuments.GetByIDAsDetailAsync(DbContext, projectDocumentID);
        if (detail == null)
        {
            return NotFound();
        }
        return Ok(detail);
    }

    [HttpPost]
    [ProjectEditAsAdminFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProjectDocumentDetail>> Create(
        [FromForm] int projectID,
        [FromForm] string displayName,
        [FromForm] string? description,
        [FromForm] int? projectDocumentTypeID,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        // Validate file extension
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
        }

        // Validate display name length
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        // Validate description length
        if (description?.Length > 1000)
        {
            return BadRequest("Description must be 1000 characters or less.");
        }

        // Check for duplicate display name
        var isUnique = await ProjectDocuments.IsDisplayNameUniqueForProjectAsync(DbContext, projectID, displayName);
        if (!isUnique)
        {
            return BadRequest($"A document with the name '{displayName}' already exists for this project.");
        }

        // Verify project exists
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        // Create file resource
        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);

        // Create project document
        var projectDocument = await ProjectDocuments.CreateAsync(
            DbContext,
            projectID,
            displayName,
            description,
            projectDocumentTypeID,
            fileResource.FileResourceID);

        var detail = await ProjectDocuments.GetByIDAsDetailAsync(DbContext, projectDocument.ProjectDocumentID);
        return CreatedAtAction(nameof(GetByID), new { projectDocumentID = projectDocument.ProjectDocumentID }, detail);
    }

    [HttpPut("{projectDocumentID}")]
    [ProjectEditFeature]
    public async Task<ActionResult<ProjectDocumentDetail>> Update(
        [FromRoute] int projectDocumentID,
        [FromBody] ProjectDocumentUpsertRequest request)
    {
        var projectDocument = await DbContext.ProjectDocuments.FindAsync(projectDocumentID);
        if (projectDocument == null)
        {
            return NotFound();
        }

        // Validate display name uniqueness
        var isUnique = await ProjectDocuments.IsDisplayNameUniqueForProjectAsync(
            DbContext,
            projectDocument.ProjectID,
            request.DisplayName,
            projectDocumentID);

        if (!isUnique)
        {
            return BadRequest($"A document with the name '{request.DisplayName}' already exists for this project.");
        }

        await ProjectDocuments.UpdateAsync(DbContext, projectDocument, request);

        var detail = await ProjectDocuments.GetByIDAsDetailAsync(DbContext, projectDocumentID);
        return Ok(detail);
    }

    [HttpDelete("{projectDocumentID}")]
    [ProjectEditFeature]
    public async Task<IActionResult> Delete([FromRoute] int projectDocumentID)
    {
        var projectDocument = await DbContext.ProjectDocuments
            .Include(pd => pd.FileResource)
            .FirstOrDefaultAsync(pd => pd.ProjectDocumentID == projectDocumentID);

        if (projectDocument == null)
        {
            return NotFound();
        }

        var fileResourceGuid = await ProjectDocuments.DeleteAsync(DbContext, projectDocument);

        // Delete from blob storage
        await fileService.DeleteFileStreamFromBlobStorageAsync(fileResourceGuid.ToString());

        return NoContent();
    }

    [HttpGet("types")]
    [AllowAnonymous]
    public ActionResult<List<ProjectDocumentTypeLookupItem>> ListTypes()
    {
        var types = ProjectDocuments.ListTypesAsLookupItem();
        return Ok(types);
    }
}
