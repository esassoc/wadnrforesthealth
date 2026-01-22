using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("taxonomy-branches")]
public class TaxonomyBranchController(
    WADNRDbContext dbContext,
    ILogger<TaxonomyBranchController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<TaxonomyBranchController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaxonomyBranchGridRow>>> List()
    {
        var rows = await TaxonomyBranches.ListAsGridRowAsync(DbContext);
        return Ok(rows);
    }

    [HttpGet("{taxonomyBranchID}")]
    [EntityNotFound(typeof(TaxonomyBranch), "taxonomyBranchID")]
    public async Task<ActionResult<TaxonomyBranchDetail>> Get([FromRoute] int taxonomyBranchID)
    {
        var entity = await TaxonomyBranches.GetByIDAsDetailAsync(DbContext, taxonomyBranchID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpGet("{taxonomyBranchID}/projects")]
    [EntityNotFound(typeof(TaxonomyBranch), "taxonomyBranchID")]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> ListProjects([FromRoute] int taxonomyBranchID)
    {
        var projects = await TaxonomyBranches.ListProjectsAsGridRowAsync(DbContext, taxonomyBranchID);
        return Ok(projects);
    }

    [HttpGet("{taxonomyBranchID}/projects/mapped-point/feature-collection")]
    [EntityNotFound(typeof(TaxonomyBranch), "taxonomyBranchID")]
    public async Task<ActionResult<FeatureCollection>> ListProjectMappedPointsFeatureCollection([FromRoute] int taxonomyBranchID)
    {
        var projectsThatShouldShowOnMap = DbContext.Projects
            .Where(x => x.ProjectType.TaxonomyBranchID == taxonomyBranchID)
            .Where(Projects.IsActiveProjectExpr)
            .AsNoTracking();

        var featureCollection = await Projects.MapProjectFeatureCollection(projectsThatShouldShowOnMap);
        return Ok(featureCollection);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<IEnumerable<TaxonomyBranchLookupItem>>> ListAsLookup()
    {
        var items = await TaxonomyBranches.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }

    [HttpPost]
    //[AdminFeature]
    public async Task<ActionResult<TaxonomyBranchDetail>> Create([FromBody] TaxonomyBranchUpsertRequest dto)
    {
        var created = await TaxonomyBranches.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { taxonomyBranchID = created.TaxonomyBranchID }, created);
    }

    [HttpPut("{taxonomyBranchID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(TaxonomyBranch), "taxonomyBranchID")]
    public async Task<ActionResult<TaxonomyBranchDetail>> Update([FromRoute] int taxonomyBranchID, [FromBody] TaxonomyBranchUpsertRequest dto)
    {
        var updated = await TaxonomyBranches.UpdateAsync(DbContext, taxonomyBranchID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{taxonomyBranchID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(TaxonomyBranch), "taxonomyBranchID")]
    public async Task<IActionResult> Delete([FromRoute] int taxonomyBranchID)
    {
        var deleted = await TaxonomyBranches.DeleteAsync(DbContext, taxonomyBranchID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
