using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("programs")]
public class ProgramController(
    WADNRDbContext dbContext,
    ILogger<ProgramController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProgramController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProgramGridRow>>> List()
    {
        var sources = await Programs.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("{programID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<ProgramDetail>> Get([FromRoute] int programID)
    {
        var entity = await Programs.GetByIDAsDetailAsync(DbContext, programID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [ProgramManageFeature]
    public async Task<ActionResult<ProgramDetail>> Create([FromBody] ProgramUpsertRequest dto)
    {
        var created = await Programs.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { programID = created.ProgramID }, created);
    }

    [HttpPut("{programID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<ProgramDetail>> Update([FromRoute] int programID, [FromBody] ProgramUpsertRequest dto)
    {
        var updated = await Programs.UpdateAsync(DbContext, programID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{programID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<IActionResult> Delete([FromRoute] int programID)
    {
        var deleted = await Programs.DeleteAsync(DbContext, programID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("{programID}/projects")]
    [AllowAnonymous]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<IEnumerable<ProjectProgramDetailGridRow>>> ListProjects([FromRoute] int programID)
    {
        var projects = await Programs.ListProjectsForProgramAsync(DbContext, programID);
        return Ok(projects);
    }

    [HttpGet("{programID}/notifications")]
    [AllowAnonymous]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<IEnumerable<ProgramNotificationGridRow>>> ListNotifications([FromRoute] int programID)
    {
        var notifications = await Programs.ListNotificationsForProgramAsync(DbContext, programID);
        return Ok(notifications);
    }
}