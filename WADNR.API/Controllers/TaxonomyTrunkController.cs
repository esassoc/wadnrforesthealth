using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("taxonomy-trunks")]
public class TaxonomyTrunkController(
    WADNRDbContext dbContext,
    ILogger<TaxonomyTrunkController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<TaxonomyTrunkController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<List<TaxonomyTrunkGridRow>>> List()
    {
        var items = await TaxonomyTrunks.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{taxonomyTrunkID}")]
    public async Task<ActionResult<TaxonomyTrunkDetail>> GetByID([FromRoute] int taxonomyTrunkID)
    {
        var item = await TaxonomyTrunks.GetByIDAsDetailAsync(DbContext, taxonomyTrunkID);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpGet("{taxonomyTrunkID}/projects")]
    public async Task<ActionResult<List<ProjectGridRow>>> ListProjects([FromRoute] int taxonomyTrunkID)
    {
        var projects = await TaxonomyTrunks.ListProjectsAsGridRowAsync(DbContext, taxonomyTrunkID);
        return Ok(projects);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<IEnumerable<TaxonomyTrunkLookupItem>>> ListAsLookup()
    {
        var items = await TaxonomyTrunks.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<TaxonomyTrunkDetail>> Create([FromBody] TaxonomyTrunkUpsertRequest dto)
    {
        var item = await TaxonomyTrunks.CreateAsync(DbContext, dto);
        return Ok(item);
    }

    [HttpPut("{taxonomyTrunkID}")]
    public async Task<ActionResult<TaxonomyTrunkDetail>> Update([FromRoute] int taxonomyTrunkID, [FromBody] TaxonomyTrunkUpsertRequest dto)
    {
        var item = await TaxonomyTrunks.UpdateAsync(DbContext, taxonomyTrunkID, dto);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpDelete("{taxonomyTrunkID}")]
    public async Task<ActionResult> Delete([FromRoute] int taxonomyTrunkID)
    {
        var deleted = await TaxonomyTrunks.DeleteAsync(DbContext, taxonomyTrunkID);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();
    }
}
