using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADNR.API.ExcelSpecs;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("fund-sources")]
public class FundSourceController(
    WADNRDbContext dbContext,
    ILogger<FundSourceController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<FundSourceController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FundSourceGridRow>>> List()
    {
        var sources = await FundSources.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("excel-download")]
    [AllowAnonymous]
    public async Task<IActionResult> ExcelDownload()
    {
        var fundSources = await FundSources.ListAsExcelRowAsync(DbContext);
        var allocations = await FundSourceAllocations.ListAsExcelRowAsync(DbContext);

        var sheets = new List<IExcelWorkbookSheetDescriptor>
        {
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Fund Sources", new FundSourceExcelSpec(), fundSources),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Fund Source Allocations", new FundSourceAllocationExcelSpec(), allocations),
        };
        var wbm = new ExcelWorkbookMaker(sheets);
        var workbook = wbm.ToXLWorkbook();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FundSources.xlsx");
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FundSourceLookupItem>>> ListLookup()
    {
        var items = await FundSources.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{fundSourceID}")]
    [AllowAnonymous]
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
    [FundSourceManageFeature]
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
    [FundSourceManageFeature]
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
    [FundSourceManageFeature]
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
    [AllowAnonymous]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceAllocationLookupItem>>> ListAllocations([FromRoute] int fundSourceID)
    {
        var allocations = await FundSources.ListAllocationsAsync(DbContext, fundSourceID);
        return Ok(allocations);
    }

    [HttpGet("{fundSourceID}/projects")]
    [AllowAnonymous]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceProjectGridRow>>> ListProjects([FromRoute] int fundSourceID)
    {
        var projects = await FundSources.ListProjectsAsync(DbContext, fundSourceID);
        return Ok(projects);
    }

    [HttpGet("{fundSourceID}/agreements")]
    [AllowAnonymous]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceAgreementGridRow>>> ListAgreements([FromRoute] int fundSourceID)
    {
        var agreements = await FundSources.ListAgreementsAsync(DbContext, fundSourceID);
        return Ok(agreements);
    }

    [HttpGet("{fundSourceID}/budget-line-items")]
    [AllowAnonymous]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceBudgetLineItemGridRow>>> ListBudgetLineItems([FromRoute] int fundSourceID)
    {
        var budgetLineItems = await FundSources.ListBudgetLineItemsAsync(DbContext, fundSourceID);
        return Ok(budgetLineItems);
    }

    [HttpGet("{fundSourceID}/files")]
    [AllowAnonymous]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceFileResourceGridRow>>> ListFiles([FromRoute] int fundSourceID)
    {
        var files = await FundSources.ListFilesAsync(DbContext, fundSourceID);
        return Ok(files);
    }

    [HttpPost("{fundSourceID}/files")]
    [FundSourceManageFeature]
    [Consumes("multipart/form-data")]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<FundSourceFileResourceGridRow>> UploadFile(
        [FromRoute] int fundSourceID,
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

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);
        await FundSources.CreateFileAsync(DbContext, fundSourceID, fileResource.FileResourceID, displayName, description);

        var files = await FundSources.ListFilesAsync(DbContext, fundSourceID);
        return Ok(files.FirstOrDefault(f => f.FileResourceID == fileResource.FileResourceID));
    }

    [HttpGet("{fundSourceID}/notes")]
    [AllowAnonymous]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceNoteGridRow>>> ListNotes([FromRoute] int fundSourceID)
    {
        var notes = await FundSources.ListNotesAsync(DbContext, fundSourceID);
        return Ok(notes);
    }

    [HttpGet("{fundSourceID}/notes-internal")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<IEnumerable<FundSourceNoteInternalGridRow>>> ListInternalNotes([FromRoute] int fundSourceID)
    {
        var notes = await FundSources.ListInternalNotesAsync(DbContext, fundSourceID);
        return Ok(notes);
    }

    // File update/delete
    [HttpPut("{fundSourceID}/files/{fundSourceFileResourceID}")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<ActionResult<FundSourceFileResourceGridRow>> UpdateFile(
        [FromRoute] int fundSourceID,
        [FromRoute] int fundSourceFileResourceID,
        [FromBody] FundSourceFileUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        if (request.Description?.Length > 1000)
        {
            return BadRequest("Description must be 1000 characters or less.");
        }

        var entity = await DbContext.FundSourceFileResources.FindAsync(fundSourceFileResourceID);
        if (entity == null || entity.FundSourceID != fundSourceID)
        {
            return NotFound();
        }

        await FundSources.UpdateFileAsync(DbContext, entity, request.DisplayName, request.Description);

        var files = await FundSources.ListFilesAsync(DbContext, fundSourceID);
        return Ok(files.FirstOrDefault(f => f.FundSourceFileResourceID == fundSourceFileResourceID));
    }

    [HttpDelete("{fundSourceID}/files/{fundSourceFileResourceID}")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<IActionResult> DeleteFile(
        [FromRoute] int fundSourceID,
        [FromRoute] int fundSourceFileResourceID)
    {
        var entity = await DbContext.FundSourceFileResources.FindAsync(fundSourceFileResourceID);
        if (entity == null || entity.FundSourceID != fundSourceID)
        {
            return NotFound();
        }

        await FundSources.DeleteFileAsync(DbContext, entity);
        return NoContent();
    }

    // Note CRUD
    [HttpPost("{fundSourceID}/notes")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<IActionResult> CreateNote(
        [FromRoute] int fundSourceID,
        [FromBody] FundSourceNoteUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest("Note is required.");
        }

        if (request.Note.Length > 8000)
        {
            return BadRequest("Note must be 8000 characters or less.");
        }

        await FundSources.CreateNoteAsync(DbContext, fundSourceID, request.Note, CallingUser.PersonID);
        return Ok();
    }

    [HttpPut("{fundSourceID}/notes/{fundSourceNoteID}")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<IActionResult> UpdateNote(
        [FromRoute] int fundSourceID,
        [FromRoute] int fundSourceNoteID,
        [FromBody] FundSourceNoteUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest("Note is required.");
        }

        if (request.Note.Length > 8000)
        {
            return BadRequest("Note must be 8000 characters or less.");
        }

        var entity = await DbContext.FundSourceNotes.FindAsync(fundSourceNoteID);
        if (entity == null || entity.FundSourceID != fundSourceID)
        {
            return NotFound();
        }

        await FundSources.UpdateNoteAsync(DbContext, entity, request.Note, CallingUser.PersonID);
        return Ok();
    }

    [HttpDelete("{fundSourceID}/notes/{fundSourceNoteID}")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<IActionResult> DeleteNote(
        [FromRoute] int fundSourceID,
        [FromRoute] int fundSourceNoteID)
    {
        var entity = await DbContext.FundSourceNotes.FindAsync(fundSourceNoteID);
        if (entity == null || entity.FundSourceID != fundSourceID)
        {
            return NotFound();
        }

        await FundSources.DeleteNoteAsync(DbContext, entity);
        return NoContent();
    }

    // Internal Note CRUD
    [HttpPost("{fundSourceID}/notes-internal")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<IActionResult> CreateNoteInternal(
        [FromRoute] int fundSourceID,
        [FromBody] FundSourceNoteUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest("Note is required.");
        }

        if (request.Note.Length > 8000)
        {
            return BadRequest("Note must be 8000 characters or less.");
        }

        await FundSources.CreateNoteInternalAsync(DbContext, fundSourceID, request.Note, CallingUser.PersonID);
        return Ok();
    }

    [HttpPut("{fundSourceID}/notes-internal/{fundSourceNoteInternalID}")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<IActionResult> UpdateNoteInternal(
        [FromRoute] int fundSourceID,
        [FromRoute] int fundSourceNoteInternalID,
        [FromBody] FundSourceNoteUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest("Note is required.");
        }

        if (request.Note.Length > 8000)
        {
            return BadRequest("Note must be 8000 characters or less.");
        }

        var entity = await DbContext.FundSourceNoteInternals.FindAsync(fundSourceNoteInternalID);
        if (entity == null || entity.FundSourceID != fundSourceID)
        {
            return NotFound();
        }

        await FundSources.UpdateNoteInternalAsync(DbContext, entity, request.Note, CallingUser.PersonID);
        return Ok();
    }

    [HttpDelete("{fundSourceID}/notes-internal/{fundSourceNoteInternalID}")]
    [FundSourceManageFeature]
    [EntityNotFound(typeof(FundSource), "fundSourceID")]
    public async Task<IActionResult> DeleteNoteInternal(
        [FromRoute] int fundSourceID,
        [FromRoute] int fundSourceNoteInternalID)
    {
        var entity = await DbContext.FundSourceNoteInternals.FindAsync(fundSourceNoteInternalID);
        if (entity == null || entity.FundSourceID != fundSourceID)
        {
            return NotFound();
        }

        await FundSources.DeleteNoteInternalAsync(DbContext, entity);
        return NoContent();
    }
}
