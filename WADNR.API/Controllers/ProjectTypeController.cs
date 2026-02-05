using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using NetTopologySuite.Features;


namespace WADNR.API.Controllers;

[ApiController]
[Route("project-types")]
public class ProjectTypeController(
    WADNRDbContext dbContext,
    ILogger<ProjectTypeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectTypeController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectTypeGridRow>>> List()
    {
        var rows = await ProjectTypes.ListAsGridRowAsync(DbContext);
        return Ok(rows);
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectTypeLookupItem>>> ListAsLookup()
    {
        var items = await ProjectTypes.ListAsLookupAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("taxonomy")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectTypeTaxonomy>>> Taxonomy()
    {
        var projectTypeTaxonomies = await ProjectTypes.ListTaxonomyAsync(DbContext);
        return Ok(projectTypeTaxonomies);
    }

    [HttpGet("{projectTypeID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(ProjectType), "projectTypeID")]
    public async Task<ActionResult<ProjectTypeDetail>> Get([FromRoute] int projectTypeID)
    {
        var entity = await ProjectTypes.GetByIDAsDetailAsync(DbContext, projectTypeID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpGet("{projectTypeID}/projects")]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<ProjectProjectTypeDetailGridRow>>> ListProjectsForProjectTypeID([FromRoute] int projectTypeID)
    {
        var projects = await Projects.ListAsProjectTypeDetailGridRowForUserAsync(DbContext, projectTypeID, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{projectTypeID}/projects/mapped-point/feature-collection")]
    [ProjectViewFeature]
    public async Task<ActionResult<FeatureCollection>> ListProjectMappedPointsFeatureCollectionForProjectTypeID(
        [FromRoute] int projectTypeID)
    {
        var visibleProjects = ProjectVisibility.ApplyVisibilityFilter(DbContext.Projects, CallingUser);
        var projectsThatShouldShowOnMap = visibleProjects
            .Where(x => x.ProjectTypeID == projectTypeID)
            .AsNoTracking();

        var featureCollection = await Projects.MapProjectFeatureCollection(projectsThatShouldShowOnMap);

        return Ok(featureCollection);
    }

    [HttpPost]
    [AdminFeature]
    public async Task<ActionResult<ProjectTypeDetail>> Create([FromBody] ProjectTypeUpsertRequest dto)
    {
        var created = await ProjectTypes.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { projectTypeID = created.ProjectTypeID }, created);
    }

    [HttpPut("{projectTypeID}")]
    [AdminFeature]
    [EntityNotFound(typeof(ProjectType), "projectTypeID")]
    public async Task<ActionResult<ProjectTypeDetail>> Update([FromRoute] int projectTypeID, [FromBody] ProjectTypeUpsertRequest dto)
    {
        var updated = await ProjectTypes.UpdateAsync(DbContext, projectTypeID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{projectTypeID}")]
    [AdminFeature]
    [EntityNotFound(typeof(ProjectType), "projectTypeID")]
    public async Task<IActionResult> Delete([FromRoute] int projectTypeID)
    {
        var deleted = await ProjectTypes.DeleteAsync(DbContext, projectTypeID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
