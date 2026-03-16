using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FileResource;
using WADNR.Models.DataTransferObjects.PriorityLandscape;

namespace WADNR.API.Controllers;

[ApiController]
[Route("priority-landscapes")]
public class PriorityLandscapeController(
    WADNRDbContext dbContext,
    ILogger<PriorityLandscapeController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<PriorityLandscapeController>(dbContext, logger, configuration)
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".zip", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".jpg", ".jpeg", ".png"
    };

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PriorityLandscapeGridRow>>> List()
    {
        var items = await PriorityLandscapes.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{priorityLandscapeID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<ActionResult<PriorityLandscapeDetail>> Get([FromRoute] int priorityLandscapeID)
    {
        var entity = await PriorityLandscapes.GetByIDAsDetailAsync(DbContext, priorityLandscapeID);
        return RequireNotNullThrowNotFound(entity, "PriorityLandscape", priorityLandscapeID);
    }

    [HttpPost]
    [AdminFeature]
    public async Task<ActionResult<PriorityLandscapeDetail>> Create([FromBody] PriorityLandscapeUpsertRequest dto)
    {
        var created = await PriorityLandscapes.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { priorityLandscapeID = created.PriorityLandscapeID }, created);
    }

    [HttpPut("{priorityLandscapeID}")]
    [AdminFeature]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<ActionResult<PriorityLandscapeDetail>> Update([FromRoute] int priorityLandscapeID, [FromBody] PriorityLandscapeUpsertRequest dto)
    {
        var updated = await PriorityLandscapes.UpdateAsync(DbContext, priorityLandscapeID, dto, CallingUser.PersonID);
        return RequireNotNullThrowNotFound(updated, "PriorityLandscape", priorityLandscapeID);
    }

    [HttpDelete("{priorityLandscapeID}")]
    [AdminFeature]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<IActionResult> Delete([FromRoute] int priorityLandscapeID)
    {
        var deleted = await PriorityLandscapes.DeleteAsync(DbContext, priorityLandscapeID);
        return DeleteOrNotFound(deleted);
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PriorityLandscapeCategoryLookupItem>>> ListCategories()
    {
        var categories = await PriorityLandscapes.ListCategoriesAsync(DbContext);
        return Ok(categories);
    }

    [HttpGet("{priorityLandscapeID}/projects")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> ListProjectsForPriorityLandscapeID([FromRoute] int priorityLandscapeID)
    {
        var linkQuery = DbContext.ProjectPriorityLandscapes
            .Where(ppl => ppl.PriorityLandscapeID == priorityLandscapeID)
            .Select(ppl => ppl.Project);

        var projects = await Projects.ListAsGridRowAsync(
            linkQuery,
            DbContext
        );

        return Ok(projects);
    }

    [HttpGet("{priorityLandscapeID}/projects/feature-collection")]
    [AllowAnonymous]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<ActionResult<FeatureCollection>> ListProjectsFeatureCollectionForPriorityLandscapeID([FromRoute] int priorityLandscapeID)
    {
        var projectQuery = DbContext.ProjectPriorityLandscapes
            .Where(ppl => ppl.PriorityLandscapeID == priorityLandscapeID)
            .Select(ppl => ppl.Project);
        var featureCollection = await Projects.MapProjectFeatureCollection(projectQuery);
        return Ok(featureCollection);
    }

    [HttpGet("{priorityLandscapeID}/file-resources")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FileResourcePriorityLandscapeDetail>>> ListFileResourcesForPriorityLandscapeID([FromRoute] int priorityLandscapeID)
    {
        var resources = await FileResources.ListForPriorityLandscapeIDAsync(DbContext, priorityLandscapeID);
        return Ok(resources);
    }

    [HttpPost("{priorityLandscapeID}/file-resources")]
    [AdminFeature]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(500_000_000)]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<ActionResult<FileResourcePriorityLandscapeDetail>> CreateFileResource(
        [FromRoute] int priorityLandscapeID,
        [FromForm] string displayName,
        [FromForm] string? description,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
        }

        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        if (description?.Length > 1000)
        {
            return BadRequest("Description must be 1000 characters or less.");
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);

        var priorityLandscapeFileResource = new PriorityLandscapeFileResource
        {
            PriorityLandscapeID = priorityLandscapeID,
            FileResourceID = fileResource.FileResourceID,
            DisplayName = displayName,
            Description = description
        };
        DbContext.PriorityLandscapeFileResources.Add(priorityLandscapeFileResource);
        await DbContext.SaveChangesAsync();

        var detail = await FileResources.ListForPriorityLandscapeIDAsync(DbContext, priorityLandscapeID);
        var created = detail.FirstOrDefault(x => x.FileResourceID == fileResource.FileResourceID);
        return Ok(created);
    }

    [HttpPut("{priorityLandscapeID}/file-resources/{priorityLandscapeFileResourceID}")]
    [AdminFeature]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<ActionResult<FileResourcePriorityLandscapeDetail>> UpdateFileResource(
        [FromRoute] int priorityLandscapeID,
        [FromRoute] int priorityLandscapeFileResourceID,
        [FromBody] PriorityLandscapeFileUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        if (request.Description?.Length > 1000)
        {
            return BadRequest("Description must be 1000 characters or less.");
        }

        var entity = await DbContext.PriorityLandscapeFileResources.FindAsync(priorityLandscapeFileResourceID);
        if (entity == null || entity.PriorityLandscapeID != priorityLandscapeID)
        {
            return NotFound();
        }

        await PriorityLandscapes.UpdateFileAsync(DbContext, entity, request.DisplayName, request.Description);

        var files = await FileResources.ListForPriorityLandscapeIDAsync(DbContext, priorityLandscapeID);
        return Ok(files.FirstOrDefault(f => f.PriorityLandscapeFileResourceID == priorityLandscapeFileResourceID));
    }

    [HttpDelete("{priorityLandscapeID}/file-resources/{priorityLandscapeFileResourceID}")]
    [AdminFeature]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<IActionResult> DeleteFileResource(
        [FromRoute] int priorityLandscapeID,
        [FromRoute] int priorityLandscapeFileResourceID)
    {
        var entity = await DbContext.PriorityLandscapeFileResources.FindAsync(priorityLandscapeFileResourceID);
        if (entity == null || entity.PriorityLandscapeID != priorityLandscapeID)
        {
            return NotFound();
        }

        await PriorityLandscapes.DeleteFileAsync(DbContext, entity);
        return NoContent();
    }
}
