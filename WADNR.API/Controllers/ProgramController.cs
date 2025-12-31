using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("programs")]
public class ProgramController(
    WADNRDbContext dbContext,
    ILogger<ProgramController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProgramController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProgramGridRow>>> List()
    {
        var sources = await Programs.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("{programID}")]
    [EntityNotFound(typeof(Program), "programID")]
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
    //[AdminFeature]
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
    //[AdminFeature]
    [EntityNotFound(typeof(Program), "programID")]
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
    //[AdminFeature]
    [EntityNotFound(typeof(Program), "programID")]
    public async Task<IActionResult> Delete([FromRoute] int programID)
    {
        var deleted = await Programs.DeleteAsync(DbContext, programID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}