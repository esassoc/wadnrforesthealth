using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("organizations")]
public class OrganizationController(
    WADNRDbContext dbContext,
    ILogger<OrganizationController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<OrganizationController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationGridRow>>> List()
    {
        var organizations = await Organizations.ListAsGridRowAsync(DbContext);
        return Ok(organizations);
    }

    [HttpGet("{organizationID}")]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<OrganizationDetail>> Get([FromRoute] int organizationID)
    {
        var entity = await Organizations.GetByIDAsDetailAsync(DbContext, organizationID);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost]
    //[AdminFeature]
    public async Task<ActionResult<OrganizationDetail>> Create([FromBody] OrganizationUpsertRequest dto)
    {
        var created = await Organizations.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { organizationID = created.OrganizationID }, created);
    }

    [HttpPut("{organizationID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<OrganizationDetail>> Update([FromRoute] int organizationID, [FromBody] OrganizationUpsertRequest dto)
    {
        var updated = await Organizations.UpdateAsync(DbContext, organizationID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{organizationID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<IActionResult> Delete([FromRoute] int organizationID)
    {
        var deleted = await Organizations.DeleteAsync(DbContext, organizationID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
