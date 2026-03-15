using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
[Route("tags")]
public class TagController(
    WADNRDbContext dbContext,
    ILogger<TagController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<TagController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TagGridRow>>> List()
    {
        var sources = await Tags.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("{tagID}")]
    [AllowAnonymous]
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
    [AdminFeature]
    public async Task<ActionResult<TagDetail>> Create([FromBody] TagUpsertRequest dto)
    {
        var validationError = await ValidateUpsertRequest(dto, null);
        if (validationError != null) return BadRequest(validationError);

        var created = await Tags.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { tagID = created.TagID }, created);
    }

    [HttpPut("{tagID}")]
    [AdminFeature]
    [EntityNotFound(typeof(Tag), "tagID")]
    public async Task<ActionResult<TagDetail>> Update([FromRoute] int tagID, [FromBody] TagUpsertRequest dto)
    {
        var validationError = await ValidateUpsertRequest(dto, tagID);
        if (validationError != null) return BadRequest(validationError);

        var updated = await Tags.UpdateAsync(DbContext, tagID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{tagID}")]
    [AdminFeature]
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

    [HttpPost("bulk-tag-projects")]
    [AdminFeature]
    public async Task<ActionResult<TagDetail>> BulkTagProjects([FromBody] BulkTagProjectsRequest dto)
    {
        var result = await Tags.BulkTagProjectsAsync(DbContext, dto);
        if (result == null)
        {
            return BadRequest();
        }
        return Ok(result);
    }

    [HttpGet("{tagID}/projects")]
    [AllowAnonymous]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<ProjectTagDetailGridRow>>> ListProjectsForTagID([FromRoute] int tagID)
    {
        var projects = await Projects.ListAsTagDetailGridRowForUserAsync(DbContext, tagID, CallingUser);
        return Ok(projects);
    }

    private async Task<string?> ValidateUpsertRequest(TagUpsertRequest request, int? excludeID)
    {
        var duplicateName = await DbContext.Tags
            .AsNoTracking()
            .AnyAsync(x => x.TagName == request.TagName && (excludeID == null || x.TagID != excludeID));
        if (duplicateName)
        {
            return "Tag Name already exists.";
        }
        return null;
    }
}
