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
[Route("fund-sources")]
public class FundSourceController(
    WADNRDbContext dbContext,
    ILogger<FundSourceController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FundSourceController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FundSourceGridRow>>> List()
    {
        var sources = await FundSources.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("{fundSourceID}")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<FundSourceDetail>> Get([FromRoute] int fundSourceID)
    {
        var entity = await FundSources.GetByIDAsDetailAsync(DbContext, fundSourceID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    //[AdminFeature]
    public async Task<ActionResult<FundSourceDetail>> Create([FromBody] FundSourceUpsertRequest dto)
    {
        var created = await FundSources.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { fundSourceID = created.FundSourceID }, created);
    }

    [HttpPut("{fundSourceID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<FundSourceDetail>> Update([FromRoute] int fundSourceID, [FromBody] FundSourceUpsertRequest dto)
    {
        var updated = await FundSources.UpdateAsync(DbContext, fundSourceID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{fundSourceID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<IActionResult> Delete([FromRoute] int fundSourceID)
    {
        var deleted = await FundSources.DeleteAsync(DbContext, fundSourceID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
