using System.Collections.Generic;
using System.Linq;
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
[Route("dnr-upland-regions")]
public class DNRUplandRegionController(
    WADNRDbContext dbContext,
    ILogger<DNRUplandRegionController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<DNRUplandRegionController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DNRUplandRegionGridRow>>> List()
    {
        var items = await DNRUplandRegions.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{dnrUplandRegionID}")]
    [EntityNotFound(typeof(DNRUplandRegion), "dnrUplandRegionID")]
    public async Task<ActionResult<DNRUplandRegionDetail>> Get([FromRoute] int dnrUplandRegionID)
    {
        var entity = await DNRUplandRegions.GetByIDAsDetailAsync(DbContext, dnrUplandRegionID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    //[AdminFeature]
    public async Task<ActionResult<DNRUplandRegionDetail>> Create([FromBody] DNRUplandRegionUpsertRequest dto)
    {
        var created = await DNRUplandRegions.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { dnrUplandRegionID = created.DNRUplandRegionID }, created);
    }

    [HttpPut("{dnrUplandRegionID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(DNRUplandRegion), "dnrUplandRegionID")]
    public async Task<ActionResult<DNRUplandRegionDetail>> Update([FromRoute] int dnrUplandRegionID, [FromBody] DNRUplandRegionUpsertRequest dto)
    {
        var updated = await DNRUplandRegions.UpdateAsync(DbContext, dnrUplandRegionID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{dnrUplandRegionID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(DNRUplandRegion), "dnrUplandRegionID")]
    public async Task<IActionResult> Delete([FromRoute] int dnrUplandRegionID)
    {
        var deleted = await DNRUplandRegions.DeleteAsync(DbContext, dnrUplandRegionID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("{dnrUplandRegionID}/projects")]
    public async Task<ActionResult<IEnumerable<ProjectDNRUplandRegionDetailGridRow>>> ListProjectsForDNRUplandRegionID([FromRoute] int dnrUplandRegionID)
    {
        var items = await Projects.ListAsDNRUplandDetailGridRowAsync(DbContext, dnrUplandRegionID);
        return Ok(items);
    }

    [HttpGet("{dnrUplandRegionID}/fund-source-allocations")]
    public async Task<ActionResult<IEnumerable<FundSourceAllocationDNRUplandRegionDetailGridRow>>> ListFundSourceAllocationsForDNRUplandRegionID([FromRoute] int dnrUplandRegionID)
    {
        var rows = await FundSourceAllocations.ListByDnrUplandRegionActiveAsync(DbContext, dnrUplandRegionID);
        return Ok(rows);
    }
}
