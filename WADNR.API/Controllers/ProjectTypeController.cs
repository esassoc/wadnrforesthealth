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
[Route("project-types")]
public class ProjectTypeController(
    WADNRDbContext dbContext,
    ILogger<ProjectTypeController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectTypeController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectTypeGridRow>>> List()
    {
        var rows = await ProjectTypes.ListAsGridRowAsync(DbContext);
        return Ok(rows);
    }

    [HttpGet("taxonomy")]
    public async Task<ActionResult<IEnumerable<ProjectTypeTaxonomy>>> Taxonomy()
    {
        var projectTypeTaxonomies = await ProjectTypes.ListTaxonomyAsync(DbContext);
        return Ok(projectTypeTaxonomies);
    }

    [HttpGet("{projectTypeID}")]
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

    [HttpPost]
    //[AdminFeature]
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
    //[AdminFeature]
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
    //[AdminFeature]
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
