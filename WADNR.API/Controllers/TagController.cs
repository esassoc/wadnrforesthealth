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
[Route("tags")]
public class TagController(
    WADNRDbContext dbContext,
    ILogger<TagController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<TagController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagGridRow>>> List()
    {
        var sources = await Tags.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("{tagID}")]
    [EntityNotFound(typeof(Tag), "tagID")]
    public async Task<ActionResult<TagDetail>> Get([FromRoute] int tagID)
    {
        var entity = await Tags.GetByIDAsDetailAsync(DbContext, tagID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    //[AdminFeature]
    public async Task<ActionResult<TagDetail>> Create([FromBody] TagUpsertRequest dto)
    {
        var created = await Tags.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { tagID = created.TagID }, created);
    }

    [HttpPut("{tagID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(Tag), "tagID")]
    public async Task<ActionResult<TagDetail>> Update([FromRoute] int tagID, [FromBody] TagUpsertRequest dto)
    {
        var updated = await Tags.UpdateAsync(DbContext, tagID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{tagID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(Tag), "tagID")]
    public async Task<IActionResult> Delete([FromRoute] int tagID)
    {
        var deleted = await Tags.DeleteAsync(DbContext, tagID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("{tagID}/projects")]
    public async Task<ActionResult<IEnumerable<ProjectTagDetailGridRow>>> ListProjectsForTagID([FromRoute] int tagID)
    {
        var projects = await Projects.ListAsTagDetailGridRowAsync(DbContext, tagID);
        return Ok(projects);
    }
}
