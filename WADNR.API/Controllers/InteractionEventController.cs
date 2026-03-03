using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using NetTopologySuite.Features;
using WADNR.Models.DataTransferObjects.FileResource;

namespace WADNR.API.Controllers;

[ApiController]
[Route("interactions-events")]
public class InteractionEventController(
    WADNRDbContext dbContext,
    ILogger<InteractionEventController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<InteractionEventController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<InteractionEventGridRow>>> List()
    {
        var sources = await InteractionEvents.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("{interactionEventID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<ActionResult<InteractionEventDetail>> Get([FromRoute] int interactionEventID)
    {
        var entity = await InteractionEvents.GetByIDAsDetailAsync(DbContext, interactionEventID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [InteractionEventEditFeature]
    public async Task<ActionResult<InteractionEventDetail>> Create([FromBody] InteractionEventUpsertRequest dto)
    {
        var created = await InteractionEvents.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { interactionEventID = created.InteractionEventID }, created);
    }

    [HttpPut("{interactionEventID}")]
    [InteractionEventEditFeature]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<ActionResult<InteractionEventDetail>> Update([FromRoute] int interactionEventID, [FromBody] InteractionEventUpsertRequest dto)
    {
        var updated = await InteractionEvents.UpdateAsync(DbContext, interactionEventID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{interactionEventID}")]
    [InteractionEventEditFeature]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<IActionResult> Delete([FromRoute] int interactionEventID)
    {
        var deleted = await InteractionEvents.DeleteAsync(DbContext, interactionEventID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("{interactionEventID}/projects")]
    [AllowAnonymous]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<ActionResult<IEnumerable<ProjectLookupItem>>> ListProjectsForInteractionEventID([FromRoute] int interactionEventID)
    {
        var projects = await InteractionEvents.ListProjectsAsLookupItemAsync(DbContext, interactionEventID);
        return Ok(projects);
    }

    [HttpGet("{interactionEventID}/contacts")]
    [AllowAnonymous]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<ActionResult<IEnumerable<PersonLookupItem>>> ListContactsForInteractionEventID([FromRoute] int interactionEventID)
    {
        var contacts = await InteractionEvents.ListContactsAsLookupItemAsync(DbContext, interactionEventID);
        return Ok(contacts);
    }

    [HttpGet("{interactionEventID}/simple-location/feature-collection")]
    [AllowAnonymous]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<ActionResult<FeatureCollection>> GetSimpleLocationForInteractionEventID([FromRoute] int interactionEventID)
    {
        var fc = await InteractionEvents.GetSimpleLocationAsFeatureCollectionAsync(DbContext, interactionEventID);
        return Ok(fc);
    }

    [HttpGet("{interactionEventID}/file-resources")]
    [AllowAnonymous]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<ActionResult<IEnumerable<FileResourceInteractionEventDetail>>> ListFileResourcesForInteractionEventID([FromRoute] int interactionEventID)
    {
        var resources = await FileResources.ListForInteractionEventIDAsync(DbContext, interactionEventID);
        return Ok(resources);
    }

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".zip", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".jpg", ".jpeg", ".png"
    };

    [HttpPost("{interactionEventID}/file-resources")]
    [InteractionEventEditFeature]
    [Consumes("multipart/form-data")]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<ActionResult<FileResourceInteractionEventDetail>> CreateFileResource(
        [FromRoute] int interactionEventID,
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

        var interactionEventFileResource = new InteractionEventFileResource
        {
            InteractionEventID = interactionEventID,
            FileResourceID = fileResource.FileResourceID,
            DisplayName = displayName,
            Description = description
        };
        DbContext.InteractionEventFileResources.Add(interactionEventFileResource);
        await DbContext.SaveChangesAsync();

        var detail = await FileResources.ListForInteractionEventIDAsync(DbContext, interactionEventID);
        var created = detail.FirstOrDefault(x => x.FileResourceID == fileResource.FileResourceID);
        return Ok(created);
    }

    [HttpPut("{interactionEventID}/simple-location")]
    [InteractionEventEditFeature]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<IActionResult> UpdateSimpleLocation(
        [FromRoute] int interactionEventID,
        [FromBody] InteractionEventLocationUpsertRequest request)
    {
        if (request.Latitude < -90 || request.Latitude > 90)
        {
            return BadRequest("Latitude must be between -90 and 90.");
        }

        if (request.Longitude < -180 || request.Longitude > 180)
        {
            return BadRequest("Longitude must be between -180 and 180.");
        }

        await InteractionEvents.UpdateLocationAsync(DbContext, interactionEventID, request.Latitude, request.Longitude);
        return NoContent();
    }

    [HttpPut("{interactionEventID}/file-resources/{interactionEventFileResourceID}")]
    [InteractionEventEditFeature]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<ActionResult<FileResourceInteractionEventDetail>> UpdateFileResource(
        [FromRoute] int interactionEventID,
        [FromRoute] int interactionEventFileResourceID,
        [FromBody] InteractionEventFileUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        if (request.Description?.Length > 1000)
        {
            return BadRequest("Description must be 1000 characters or less.");
        }

        var entity = await DbContext.InteractionEventFileResources.FindAsync(interactionEventFileResourceID);
        if (entity == null || entity.InteractionEventID != interactionEventID)
        {
            return NotFound();
        }

        await InteractionEvents.UpdateFileAsync(DbContext, entity, request.DisplayName, request.Description);

        var files = await FileResources.ListForInteractionEventIDAsync(DbContext, interactionEventID);
        return Ok(files.FirstOrDefault(f => f.InteractionEventFileResourceID == interactionEventFileResourceID));
    }

    [HttpDelete("{interactionEventID}/file-resources/{interactionEventFileResourceID}")]
    [InteractionEventEditFeature]
    [EntityNotFound(typeof(InteractionEvent), "interactionEventID")]
    public async Task<IActionResult> DeleteFileResource(
        [FromRoute] int interactionEventID,
        [FromRoute] int interactionEventFileResourceID)
    {
        var entity = await DbContext.InteractionEventFileResources.FindAsync(interactionEventFileResourceID);
        if (entity == null || entity.InteractionEventID != interactionEventID)
        {
            return NotFound();
        }

        await InteractionEvents.DeleteFileAsync(DbContext, entity);
        return NoContent();
    }
}
