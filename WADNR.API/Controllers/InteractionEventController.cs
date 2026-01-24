using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
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
[Route("interaction-events")]
public class InteractionEventController(
    WADNRDbContext dbContext,
    ILogger<InteractionEventController> logger,
    IOptions<WADNRConfiguration> configuration)
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
    [ProjectEditFeature]
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
    [ProjectEditFeature]
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
    [ProjectEditFeature]
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
}
