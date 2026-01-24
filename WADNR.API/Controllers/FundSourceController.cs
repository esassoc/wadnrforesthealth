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
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FundSourceController>(dbContext, logger, configuration)
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

    [HttpGet("{fundSourceID}/allocations")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceAllocationLookupItem>>> ListAllocations([FromRoute] int fundSourceID)
    {
        var allocations = await FundSources.ListAllocationsAsync(DbContext, fundSourceID);
        return Ok(allocations);
    }

    [HttpGet("{fundSourceID}/projects")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceProjectGridRow>>> ListProjects([FromRoute] int fundSourceID)
    {
        var projects = await FundSources.ListProjectsAsync(DbContext, fundSourceID);
        return Ok(projects);
    }

    [HttpGet("{fundSourceID}/agreements")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceAgreementGridRow>>> ListAgreements([FromRoute] int fundSourceID)
    {
        var agreements = await FundSources.ListAgreementsAsync(DbContext, fundSourceID);
        return Ok(agreements);
    }

    [HttpGet("{fundSourceID}/budget-line-items")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceBudgetLineItemGridRow>>> ListBudgetLineItems([FromRoute] int fundSourceID)
    {
        var budgetLineItems = await FundSources.ListBudgetLineItemsAsync(DbContext, fundSourceID);
        return Ok(budgetLineItems);
    }

    [HttpGet("{fundSourceID}/files")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceFileResourceGridRow>>> ListFiles([FromRoute] int fundSourceID)
    {
        var files = await FundSources.ListFilesAsync(DbContext, fundSourceID);
        return Ok(files);
    }

    [HttpGet("{fundSourceID}/notes")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceNoteGridRow>>> ListNotes([FromRoute] int fundSourceID)
    {
        var notes = await FundSources.ListNotesAsync(DbContext, fundSourceID);
        return Ok(notes);
    }

    [HttpGet("{fundSourceID}/notes-internal")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceNoteInternalGridRow>>> ListInternalNotes([FromRoute] int fundSourceID)
    {
        var notes = await FundSources.ListInternalNotesAsync(DbContext, fundSourceID);
        return Ok(notes);
    }
}
