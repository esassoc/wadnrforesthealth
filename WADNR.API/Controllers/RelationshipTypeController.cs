using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("relationship-types")]
public class RelationshipTypeController(
    WADNRDbContext dbContext,
    ILogger<RelationshipTypeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<RelationshipTypeController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RelationshipTypeGridRow>>> List()
    {
        var items = await RelationshipTypes.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RelationshipTypeLookupItem>>> ListLookup()
    {
        var items = await RelationshipTypes.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RelationshipTypeSummary>>> ListSummary()
    {
        var items = await RelationshipTypes.ListAsSummaryAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{relationshipTypeID}")]
    [AdminFeature]
    public async Task<ActionResult<RelationshipTypeGridRow>> Get([FromRoute] int relationshipTypeID)
    {
        var entity = await RelationshipTypes.GetByIDAsGridRowAsync(DbContext, relationshipTypeID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [AdminFeature]
    public async Task<ActionResult<RelationshipTypeGridRow>> Create([FromBody] RelationshipTypeUpsertRequest dto)
    {
        var validationError = await RelationshipTypes.ValidateUpsertAsync(DbContext, dto);
        if (validationError != null)
        {
            return BadRequest(new { message = validationError });
        }

        var created = await RelationshipTypes.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { relationshipTypeID = created.RelationshipTypeID }, created);
    }

    [HttpPut("{relationshipTypeID}")]
    [AdminFeature]
    public async Task<ActionResult<RelationshipTypeGridRow>> Update([FromRoute] int relationshipTypeID, [FromBody] RelationshipTypeUpsertRequest dto)
    {
        var validationError = await RelationshipTypes.ValidateUpsertAsync(DbContext, dto, relationshipTypeID);
        if (validationError != null)
        {
            return BadRequest(new { message = validationError });
        }

        var updated = await RelationshipTypes.UpdateAsync(DbContext, relationshipTypeID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{relationshipTypeID}")]
    [AdminFeature]
    public async Task<IActionResult> Delete([FromRoute] int relationshipTypeID)
    {
        var deleted = await RelationshipTypes.DeleteAsync(DbContext, relationshipTypeID);
        if (!deleted)
        {
            return BadRequest("Cannot delete a relationship type that has project organizations assigned to it.");
        }
        return NoContent();
    }
}
