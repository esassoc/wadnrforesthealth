using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.API.Controllers;

[ApiController]
[Route("fund-source-allocation-notes")]
public class FundSourceAllocationNoteController(
    WADNRDbContext dbContext,
    ILogger<FundSourceAllocationNoteController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FundSourceAllocationNoteController>(dbContext, logger, configuration)
{
    [HttpGet("{fundSourceAllocationNoteID}")]
    [AllowAnonymous]
    public async Task<ActionResult<FundSourceAllocationNoteDetail>> GetByID([FromRoute] int fundSourceAllocationNoteID)
    {
        var detail = await FundSourceAllocationNotes.GetByIDAsDetailAsync(DbContext, fundSourceAllocationNoteID);
        if (detail == null)
        {
            return NotFound();
        }
        return Ok(detail);
    }

    [HttpPost]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceAllocationNoteDetail>> Create([FromBody] FundSourceAllocationNoteUpsertRequest request)
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

        var entity = await FundSourceAllocationNotes.CreateAsync(
            DbContext, request.FundSourceAllocationID, request.Note, CallingUser.PersonID);

        var detail = await FundSourceAllocationNotes.GetByIDAsDetailAsync(DbContext, entity.FundSourceAllocationNoteID);
        return CreatedAtAction(nameof(GetByID), new { fundSourceAllocationNoteID = entity.FundSourceAllocationNoteID }, detail);
    }

    [HttpPut("{fundSourceAllocationNoteID}")]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceAllocationNoteDetail>> Update(
        [FromRoute] int fundSourceAllocationNoteID,
        [FromBody] FundSourceAllocationNoteUpsertRequest request)
    {
        var entity = await DbContext.FundSourceAllocationNotes.FindAsync(fundSourceAllocationNoteID);
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

        await FundSourceAllocationNotes.UpdateAsync(DbContext, entity, request.Note, CallingUser.PersonID);

        var detail = await FundSourceAllocationNotes.GetByIDAsDetailAsync(DbContext, fundSourceAllocationNoteID);
        return Ok(detail);
    }

    [HttpDelete("{fundSourceAllocationNoteID}")]
    [FundSourceManageFeature]
    public async Task<IActionResult> Delete([FromRoute] int fundSourceAllocationNoteID)
    {
        var entity = await DbContext.FundSourceAllocationNotes.FindAsync(fundSourceAllocationNoteID);
        if (entity == null)
        {
            return NotFound();
        }

        await FundSourceAllocationNotes.DeleteAsync(DbContext, entity);
        return NoContent();
    }
}
