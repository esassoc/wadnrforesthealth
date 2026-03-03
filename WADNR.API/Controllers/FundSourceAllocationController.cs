using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.API.Controllers;

[ApiController]
[Route("fund-source-allocations")]
public class FundSourceAllocationController(
    WADNRDbContext dbContext,
    ILogger<FundSourceAllocationController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<FundSourceAllocationController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationGridRow>>> List()
    {
        var fundSourceAllocations = await FundSourceAllocations.ListAsGridRowAsync(DbContext);
        return Ok(fundSourceAllocations);
    }

    [HttpGet("{fundSourceAllocationID}")]
    [AllowAnonymous]
    public async Task<ActionResult<FundSourceAllocationDetail>> GetByID([FromRoute] int fundSourceAllocationID)
    {
        var fundSourceAllocation = await FundSourceAllocations.GetByIDAsDetailAsync(DbContext, fundSourceAllocationID);
        if (fundSourceAllocation == null)
        {
            return NotFound();
        }
        return Ok(fundSourceAllocation);
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationLookupItem>>> ListLookup()
    {
        var fundSourceAllocations = await FundSourceAllocations.ListAsLookupItemAsync(DbContext);
        return Ok(fundSourceAllocations);
    }

    [HttpPost]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceAllocationDetail>> Create([FromBody] FundSourceAllocationUpsertRequest dto)
    {
        var created = await FundSourceAllocations.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(GetByID), new { fundSourceAllocationID = created.FundSourceAllocationID }, created);
    }

    [HttpPut("{fundSourceAllocationID}")]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceAllocationDetail>> Update(
        [FromRoute] int fundSourceAllocationID,
        [FromBody] FundSourceAllocationUpsertRequest request)
    {
        var allocation = await DbContext.FundSourceAllocations.FindAsync(fundSourceAllocationID);
        if (allocation == null)
        {
            return NotFound();
        }

        await FundSourceAllocations.UpdateAsync(DbContext, fundSourceAllocationID, request, CallingUser.PersonID);

        var detail = await FundSourceAllocations.GetByIDAsDetailAsync(DbContext, fundSourceAllocationID);
        return Ok(detail);
    }

    [HttpDelete("{fundSourceAllocationID}")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSourceAllocation), "fundSourceAllocationID")]
    public async Task<IActionResult> Delete([FromRoute] int fundSourceAllocationID)
    {
        var deleted = await FundSourceAllocations.DeleteAsync(DbContext, fundSourceAllocationID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("{fundSourceAllocationID}/notes")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationNoteGridRow>>> ListNotes(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocationNotes.ListForAllocationAsGridRowAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpGet("{fundSourceAllocationID}/notes-internal")]
    [FundSourceManageFeature]
    public async Task<ActionResult<List<FundSourceAllocationNoteInternalGridRow>>> ListNotesInternal(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocationNoteInternals.ListForAllocationAsGridRowAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpGet("{fundSourceAllocationID}/budget-line-items")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationBudgetLineItemGridRow>>> ListBudgetLineItems(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocations.ListBudgetLineItemsAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpPut("{fundSourceAllocationID}/budget-line-items")]
    [FundSourceManageFeature]
    public async Task<ActionResult<List<FundSourceAllocationBudgetLineItemGridRow>>> SaveBudgetLineItems(
        [FromRoute] int fundSourceAllocationID,
        [FromBody] FundSourceAllocationBudgetLineItemUpsertRequest request)
    {
        var items = await FundSourceAllocations.SaveBudgetLineItemsAsync(DbContext, fundSourceAllocationID, request);
        return Ok(items);
    }

    [HttpGet("{fundSourceAllocationID}/projects")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationProjectGridRow>>> ListProjects(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocations.ListProjectsAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpGet("{fundSourceAllocationID}/agreements")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationAgreementGridRow>>> ListAgreements(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocations.ListAgreementsAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpGet("{fundSourceAllocationID}/files")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationFileGridRow>>> ListFiles(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocations.ListFilesAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpPost("{fundSourceAllocationID}/files")]
    [FundSourceManageFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FundSourceAllocationFileGridRow>> UploadFile(
        [FromRoute] int fundSourceAllocationID,
        [FromForm] string displayName,
        [FromForm] string? description,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        if (description?.Length > 1000)
        {
            return BadRequest("Description must be 1000 characters or less.");
        }

        var allocation = await DbContext.FundSourceAllocations.FindAsync(fundSourceAllocationID);
        if (allocation == null)
        {
            return NotFound();
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);
        await FundSourceAllocations.CreateFileAsync(DbContext, fundSourceAllocationID, fileResource.FileResourceID, displayName, description);

        var files = await FundSourceAllocations.ListFilesAsync(DbContext, fundSourceAllocationID);
        return Ok(files.FirstOrDefault(f => f.FileResourceID == fileResource.FileResourceID));
    }

    [HttpPut("{fundSourceAllocationID}/files/{fundSourceAllocationFileResourceID}")]
    [FundSourceManageFeature]
    public async Task<ActionResult<FundSourceAllocationFileGridRow>> UpdateFile(
        [FromRoute] int fundSourceAllocationID,
        [FromRoute] int fundSourceAllocationFileResourceID,
        [FromBody] FundSourceAllocationFileUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        if (request.Description?.Length > 1000)
        {
            return BadRequest("Description must be 1000 characters or less.");
        }

        var entity = await DbContext.FundSourceAllocationFileResources.FindAsync(fundSourceAllocationFileResourceID);
        if (entity == null || entity.FundSourceAllocationID != fundSourceAllocationID)
        {
            return NotFound();
        }

        await FundSourceAllocations.UpdateFileAsync(DbContext, entity, request.DisplayName, request.Description);

        var files = await FundSourceAllocations.ListFilesAsync(DbContext, fundSourceAllocationID);
        return Ok(files.FirstOrDefault(f => f.FundSourceAllocationFileResourceID == fundSourceAllocationFileResourceID));
    }

    [HttpDelete("{fundSourceAllocationID}/files/{fundSourceAllocationFileResourceID}")]
    [FundSourceManageFeature]
    public async Task<IActionResult> DeleteFile(
        [FromRoute] int fundSourceAllocationID,
        [FromRoute] int fundSourceAllocationFileResourceID)
    {
        var entity = await DbContext.FundSourceAllocationFileResources.FindAsync(fundSourceAllocationFileResourceID);
        if (entity == null || entity.FundSourceAllocationID != fundSourceAllocationID)
        {
            return NotFound();
        }

        await FundSourceAllocations.DeleteFileAsync(DbContext, entity);
        return NoContent();
    }

    [HttpGet("{fundSourceAllocationID}/expenditures")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationExpenditureGridRow>>> ListExpenditures(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocations.ListExpendituresAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpGet("{fundSourceAllocationID}/expenditure-summary")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationExpenditureSummary>>> ListExpenditureSummary(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocations.ListExpenditureSummaryAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpGet("{fundSourceAllocationID}/change-logs")]
    [FundSourceManageFeature]
    public async Task<ActionResult<List<FundSourceAllocationChangeLogGridRow>>> ListChangeLogs(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocations.ListChangeLogsAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpGet("{fundSourceAllocationID}/program-index-project-codes")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationProgramIndexProjectCodeItem>>> ListProgramIndexProjectCodes(
        [FromRoute] int fundSourceAllocationID)
    {
        var items = await FundSourceAllocations.ListProgramIndexProjectCodesAsync(DbContext, fundSourceAllocationID);
        return Ok(items);
    }

    [HttpPut("{fundSourceAllocationID}/program-index-project-codes")]
    [FundSourceManageFeature]
    public async Task<ActionResult<List<FundSourceAllocationProgramIndexProjectCodeItem>>> SaveProgramIndexProjectCodes(
        [FromRoute] int fundSourceAllocationID,
        [FromBody] FundSourceAllocationProgramIndexProjectCodeSaveRequest request)
    {
        var items = await FundSourceAllocations.SaveProgramIndexProjectCodesAsync(
            DbContext, fundSourceAllocationID, request);
        return Ok(items);
    }
}
