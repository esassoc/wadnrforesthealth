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
[Route("organization-types")]
public class OrganizationTypeController(
    WADNRDbContext dbContext,
    ILogger<OrganizationTypeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<OrganizationTypeController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationTypeGridRow>>> List()
    {
        var items = await OrganizationTypes.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationTypeLookupItem>>> ListLookup()
    {
        var items = await OrganizationTypes.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{organizationTypeID}")]
    [AdminFeature]
    public async Task<ActionResult<OrganizationTypeGridRow>> Get([FromRoute] int organizationTypeID)
    {
        var entity = await OrganizationTypes.GetByIDAsGridRowAsync(DbContext, organizationTypeID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [AdminFeature]
    public async Task<ActionResult<OrganizationTypeGridRow>> Create([FromBody] OrganizationTypeUpsertRequest dto)
    {
        var created = await OrganizationTypes.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { organizationTypeID = created.OrganizationTypeID }, created);
    }

    [HttpPut("{organizationTypeID}")]
    [AdminFeature]
    public async Task<ActionResult<OrganizationTypeGridRow>> Update([FromRoute] int organizationTypeID, [FromBody] OrganizationTypeUpsertRequest dto)
    {
        var updated = await OrganizationTypes.UpdateAsync(DbContext, organizationTypeID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{organizationTypeID}")]
    [AdminFeature]
    public async Task<IActionResult> Delete([FromRoute] int organizationTypeID)
    {
        var deleted = await OrganizationTypes.DeleteAsync(DbContext, organizationTypeID);
        if (!deleted)
        {
            return BadRequest("Cannot delete an organization type that has organizations assigned to it.");
        }
        return NoContent();
    }
}
