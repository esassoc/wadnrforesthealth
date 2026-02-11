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
using WADNR.Models.DataTransferObjects.FocusArea;

namespace WADNR.API.Controllers;

[ApiController]
[Route("dnr-upland-regions")]
public class DNRUplandRegionController(
    WADNRDbContext dbContext,
    ILogger<DNRUplandRegionController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<DNRUplandRegionController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DNRUplandRegionGridRow>>> List()
    {
        var items = await DNRUplandRegions.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{dnrUplandRegionID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(DNRUplandRegion), "dnrUplandRegionID")]
    public async Task<ActionResult<DNRUplandRegionDetail>> Get([FromRoute] int dnrUplandRegionID)
    {
        var entity = await DNRUplandRegions.GetByIDAsDetailAsync(DbContext, dnrUplandRegionID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [AdminFeature]
    public async Task<ActionResult<DNRUplandRegionDetail>> Create([FromBody] DNRUplandRegionUpsertRequest dto)
    {
        var created = await DNRUplandRegions.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { dnrUplandRegionID = created.DNRUplandRegionID }, created);
    }

    [HttpPut("{dnrUplandRegionID}")]
    [AdminFeature]
    [EntityNotFound(typeof(DNRUplandRegion), "dnrUplandRegionID")]
    public async Task<ActionResult<DNRUplandRegionDetail>> Update([FromRoute] int dnrUplandRegionID, [FromBody] DNRUplandRegionUpsertRequest dto)
    {
        var updated = await DNRUplandRegions.UpdateAsync(DbContext, dnrUplandRegionID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{dnrUplandRegionID}")]
    [AdminFeature]
    [EntityNotFound(typeof(DNRUplandRegion), "dnrUplandRegionID")]
    public async Task<IActionResult> Delete([FromRoute] int dnrUplandRegionID)
    {
        var deleted = await DNRUplandRegions.DeleteAsync(DbContext, dnrUplandRegionID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("{dnrUplandRegionID}/projects")]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<ProjectDNRUplandRegionDetailGridRow>>> ListProjectsForDNRUplandRegionID([FromRoute] int dnrUplandRegionID)
    {
        var items = await Projects.ListAsDNRUplandDetailGridRowForUserAsync(DbContext, dnrUplandRegionID, CallingUser);
        return Ok(items);
    }

    [HttpGet("{dnrUplandRegionID}/projects/feature-collection")]
    [AllowAnonymous]
    [EntityNotFound(typeof(DNRUplandRegion), "dnrUplandRegionID")]
    public async Task<ActionResult<FeatureCollection>> ListProjectsFeatureCollectionForDNRUplandRegionID([FromRoute] int dnrUplandRegionID)
    {
        var projectQuery = DbContext.ProjectRegions
            .Where(pr => pr.DNRUplandRegionID == dnrUplandRegionID)
            .Select(pr => pr.Project);
        var featureCollection = await Projects.MapProjectFeatureCollection(projectQuery);
        return Ok(featureCollection);
    }

    [HttpGet("{dnrUplandRegionID}/focus-areas")]
    [NormalUserFeature]
    public async Task<ActionResult<List<FocusAreaGridRow>>> ListFocusAreas([FromRoute] int dnrUplandRegionID)
    {
        var focusAreas = await FocusAreas.ListForRegionAsGridRowAsync(DbContext, dnrUplandRegionID);
        return Ok(focusAreas);
    }

    [HttpGet("{dnrUplandRegionID}/fund-source-allocations")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FundSourceAllocationDNRUplandRegionDetailGridRow>>> ListFundSourceAllocationsForDNRUplandRegionID([FromRoute] int dnrUplandRegionID)
    {
        var rows = await FundSourceAllocations.ListByDnrUplandRegionActiveAsync(DbContext, dnrUplandRegionID);
        return Ok(rows);
    }
}
