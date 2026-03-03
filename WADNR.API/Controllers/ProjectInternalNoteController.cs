using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("project-internal-notes")]
public class ProjectInternalNoteController(
    WADNRDbContext dbContext,
    ILogger<ProjectInternalNoteController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectInternalNoteController>(dbContext, logger, configuration)
{
    [HttpGet("{projectInternalNoteID}")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<ProjectInternalNoteDetail>> GetByID([FromRoute] int projectInternalNoteID)
    {
        var detail = await ProjectInternalNotes.GetByIDAsDetailAsync(DbContext, projectInternalNoteID);
        if (detail == null)
        {
            return NotFound();
        }
        return Ok(detail);
    }

    [HttpPost]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<ProjectInternalNoteDetail>> Create([FromBody] ProjectInternalNoteUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest("Note is required.");
        }

        if (request.Note.Length > 8000)
        {
            return BadRequest("Note must be 8000 characters or less.");
        }

        var project = await DbContext.Projects.FindAsync(request.ProjectID);
        if (project == null)
        {
            return NotFound($"Project with ID {request.ProjectID} not found.");
        }

        var projectInternalNote = await ProjectInternalNotes.CreateAsync(
            DbContext,
            request.ProjectID,
            request.Note,
            CallingUser.PersonID);

        var detail = await ProjectInternalNotes.GetByIDAsDetailAsync(DbContext, projectInternalNote.ProjectInternalNoteID);
        return CreatedAtAction(nameof(GetByID), new { projectInternalNoteID = projectInternalNote.ProjectInternalNoteID }, detail);
    }

    [HttpPut("{projectInternalNoteID}")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<ProjectInternalNoteDetail>> Update(
        [FromRoute] int projectInternalNoteID,
        [FromBody] ProjectInternalNoteUpsertRequest request)
    {
        var projectInternalNote = await DbContext.ProjectInternalNotes.FindAsync(projectInternalNoteID);
        if (projectInternalNote == null)
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

        await ProjectInternalNotes.UpdateAsync(DbContext, projectInternalNote, request.Note, CallingUser.PersonID);

        var detail = await ProjectInternalNotes.GetByIDAsDetailAsync(DbContext, projectInternalNoteID);
        return Ok(detail);
    }

    [HttpDelete("{projectInternalNoteID}")]
    [ProjectEditAsAdminFeature]
    public async Task<IActionResult> Delete([FromRoute] int projectInternalNoteID)
    {
        var projectInternalNote = await DbContext.ProjectInternalNotes.FindAsync(projectInternalNoteID);
        if (projectInternalNote == null)
        {
            return NotFound();
        }

        await ProjectInternalNotes.DeleteAsync(DbContext, projectInternalNote);
        return NoContent();
    }
}
