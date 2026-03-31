using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.API.Controllers;

[ApiController]
[Route("fund-source-allocation-notes-internal")]
public class FundSourceAllocationNoteInternalController(
    WADNRDbContext dbContext,
    ILogger<FundSourceAllocationNoteInternalController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FundSourceAllocationNoteInternalController>(dbContext, logger, configuration)
{
    [HttpGet("{fundSourceAllocationNoteInternalID}")]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceAllocationNoteInternalDetail>> GetByID([FromRoute] int fundSourceAllocationNoteInternalID)
    {
        var detail = await FundSourceAllocationNoteInternals.GetByIDAsDetailAsync(DbContext, fundSourceAllocationNoteInternalID);
        if (detail == null)
        {
            return NotFound();
        }
        return Ok(detail);
    }

    [HttpPost]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceAllocationNoteInternalDetail>> Create([FromBody] FundSourceAllocationNoteInternalUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest("Note is required.");
        }

        if (request.Note.Length > 8000)
        {
            return BadRequest("Note must be 8000 characters or less.");
        }

        var allocation = await DbContext.FundSourceAllocations.FindAsync(request.FundSourceAllocationID);
        if (allocation == null)
        {
            return NotFound($"Fund Source Allocation with ID {request.FundSourceAllocationID} not found.");
        }

        var entity = await FundSourceAllocationNoteInternals.CreateAsync(
            DbContext, request.FundSourceAllocationID, request.Note, CallingUser.PersonID);

        var detail = await FundSourceAllocationNoteInternals.GetByIDAsDetailAsync(DbContext, entity.FundSourceAllocationNoteInternalID);
        return CreatedAtAction(nameof(GetByID), new { fundSourceAllocationNoteInternalID = entity.FundSourceAllocationNoteInternalID }, detail);
    }

    [HttpPut("{fundSourceAllocationNoteInternalID}")]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceAllocationNoteInternalDetail>> Update(
        [FromRoute] int fundSourceAllocationNoteInternalID,
        [FromBody] FundSourceAllocationNoteInternalUpsertRequest request)
    {
        var entity = await DbContext.FundSourceAllocationNoteInternals.FindAsync(fundSourceAllocationNoteInternalID);
        if (entity == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest("Note is required.");
        }

        if (request.Note.Length > 8000)
        {
            return BadRequest("Note must be 8000 characters or less.");
        }

        await FundSourceAllocationNoteInternals.UpdateAsync(DbContext, entity, request.Note, CallingUser.PersonID);

        var detail = await FundSourceAllocationNoteInternals.GetByIDAsDetailAsync(DbContext, fundSourceAllocationNoteInternalID);
        return Ok(detail);
    }

    [HttpDelete("{fundSourceAllocationNoteInternalID}")]
    [FundSourceManageFeature]
    public async Task<IActionResult> Delete([FromRoute] int fundSourceAllocationNoteInternalID)
    {
        var entity = await DbContext.FundSourceAllocationNoteInternals.FindAsync(fundSourceAllocationNoteInternalID);
        if (entity == null)
        {
            return NotFound();
        }

        await FundSourceAllocationNoteInternals.DeleteAsync(DbContext, entity);
        return NoContent();
    }
}
