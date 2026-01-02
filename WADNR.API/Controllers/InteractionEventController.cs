using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("interaction-events")]
public class InteractionEventController(
    WADNRDbContext dbContext,
    ILogger<InteractionEventController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<InteractionEventController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InteractionEventGridRow>>> List()
    {
        var sources = await InteractionEvents.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("{interactionEventID}")]
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
    //[AdminFeature]
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
    //[AdminFeature]
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
    //[AdminFeature]
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
}
