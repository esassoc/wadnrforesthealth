using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FileResource;

namespace WADNR.API.Controllers;

[ApiController]
[Route("priority-landscapes")]
public class PriorityLandscapeController(
    WADNRDbContext dbContext,
    ILogger<PriorityLandscapeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<PriorityLandscapeController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PriorityLandscapeGridRow>>> List()
    {
        var items = await PriorityLandscapes.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{priorityLandscapeID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<ActionResult<PriorityLandscapeDetail>> Get([FromRoute] int priorityLandscapeID)
    {
        var entity = await PriorityLandscapes.GetByIDAsDetailAsync(DbContext, priorityLandscapeID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [AdminFeature]
    public async Task<ActionResult<PriorityLandscapeDetail>> Create([FromBody] PriorityLandscapeUpsertRequest dto)
    {
        var created = await PriorityLandscapes.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { priorityLandscapeID = created.PriorityLandscapeID }, created);
    }

    [HttpPut("{priorityLandscapeID}")]
    [AdminFeature]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<ActionResult<PriorityLandscapeDetail>> Update([FromRoute] int priorityLandscapeID, [FromBody] PriorityLandscapeUpsertRequest dto)
    {
        var updated = await PriorityLandscapes.UpdateAsync(DbContext, priorityLandscapeID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{priorityLandscapeID}")]
    [AdminFeature]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<IActionResult> Delete([FromRoute] int priorityLandscapeID)
    {
        var deleted = await PriorityLandscapes.DeleteAsync(DbContext, priorityLandscapeID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("{priorityLandscapeID}/projects")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> ListProjectsForPriorityLandscapeID([FromRoute] int priorityLandscapeID)
    {
        var linkQuery = DbContext.ProjectPriorityLandscapes
            .Where(ppl => ppl.PriorityLandscapeID == priorityLandscapeID)
            .Select(ppl => ppl.Project);

        var projects = await Projects.ListAsGridRowAsync(
            linkQuery,
            DbContext
        );

        return Ok(projects);
    }

    [HttpGet("{priorityLandscapeID}/projects/feature-collection")]
    [AllowAnonymous]
    [EntityNotFound(typeof(PriorityLandscape), "priorityLandscapeID")]
    public async Task<ActionResult<FeatureCollection>> ListProjectsFeatureCollectionForPriorityLandscapeID([FromRoute] int priorityLandscapeID)
    {
        var projectQuery = DbContext.ProjectPriorityLandscapes
            .Where(ppl => ppl.PriorityLandscapeID == priorityLandscapeID)
            .Select(ppl => ppl.Project);
        var featureCollection = await Projects.MapProjectFeatureCollection(projectQuery);
        return Ok(featureCollection);
    }

    [HttpGet("{priorityLandscapeID}/file-resources")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FileResourcePriorityLandscapeDetail>>> ListFileResourcesForPriorityLandscapeID([FromRoute] int priorityLandscapeID)
    {
        var resources = await FileResources.ListForPriorityLandscapeIDAsync(DbContext, priorityLandscapeID);
        return Ok(resources);
    }
}
