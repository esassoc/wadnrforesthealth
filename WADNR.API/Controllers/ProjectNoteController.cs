using Microsoft.AspNetCore.Authorization;
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
[Route("project-notes")]
public class ProjectNoteController(
    WADNRDbContext dbContext,
    ILogger<ProjectNoteController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectNoteController>(dbContext, logger, configuration)
{
    [HttpGet("{projectNoteID}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProjectNoteDetail>> GetByID([FromRoute] int projectNoteID)
    {
        var detail = await ProjectNotes.GetByIDAsDetailAsync(DbContext, projectNoteID);
        if (detail == null)
        {
            return NotFound();
        }
        return Ok(detail);
    }

    [HttpPost]
    [ProjectEditFeature]
    public async Task<ActionResult<ProjectNoteDetail>> Create([FromBody] ProjectNoteUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest("Note is required.");
        }

        if (request.Note.Length > 8000)
        {
            return BadRequest("Note must be 8000 characters or less.");
        }

        // Verify project exists
        var project = await DbContext.Projects.FindAsync(request.ProjectID);
        if (project == null)
        {
            return NotFound($"Project with ID {request.ProjectID} not found.");
        }

        var projectNote = await ProjectNotes.CreateAsync(
            DbContext,
            request.ProjectID,
            request.Note,
            CallingUser.PersonID);

        var detail = await ProjectNotes.GetByIDAsDetailAsync(DbContext, projectNote.ProjectNoteID);
        return CreatedAtAction(nameof(GetByID), new { projectNoteID = projectNote.ProjectNoteID }, detail);
    }

    [HttpPut("{projectNoteID}")]
    [ProjectEditFeature]
    public async Task<ActionResult<ProjectNoteDetail>> Update(
        [FromRoute] int projectNoteID,
        [FromBody] ProjectNoteUpsertRequest request)
    {
        var projectNote = await DbContext.ProjectNotes.FindAsync(projectNoteID);
        if (projectNote == null)
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

        await ProjectNotes.UpdateAsync(DbContext, projectNote, request.Note, CallingUser.PersonID);

        var detail = await ProjectNotes.GetByIDAsDetailAsync(DbContext, projectNoteID);
        return Ok(detail);
    }

    [HttpDelete("{projectNoteID}")]
    [ProjectEditFeature]
    public async Task<IActionResult> Delete([FromRoute] int projectNoteID)
    {
        var projectNote = await DbContext.ProjectNotes.FindAsync(projectNoteID);
        if (projectNote == null)
        {
            return NotFound();
        }

        await ProjectNotes.DeleteAsync(DbContext, projectNote);
        return NoContent();
    }
}
