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
[Route("projects")]
public class ProjectController(
    WADNRDbContext dbContext,
    ILogger<ProjectController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> List()
    {
        var projects = await Projects.ListAsGridRowAsync(DbContext);
        return Ok(projects);
    }

    [HttpGet("{projectID}")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectDetail>> Get([FromRoute] int projectID)
    {
        var entity = await Projects.GetByIDAsDetailAsync(DbContext, projectID);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpGet("{projectID}/map-popup")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectMapPopup>> GetAsMapPopup([FromRoute] int projectID)
    {
        var entity = await Projects.GetByIDAsMapPopupAsync(DbContext, projectID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    //[AdminFeature]
    public async Task<ActionResult<ProjectDetail>> Create([FromBody] ProjectUpsertRequest dto)
    {
        var created = await Projects.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { projectID = created.ProjectID }, created);
    }

    [HttpPut("{projectID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectDetail>> Update([FromRoute] int projectID, [FromBody] ProjectUpsertRequest dto)
    {
        var updated = await Projects.UpdateAsync(DbContext, projectID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{projectID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> Delete([FromRoute] int projectID)
    {
        var deleted = await Projects.DeleteAsync(DbContext, projectID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("mapped-point/feature-collection")]
    public async Task<ActionResult<FeatureCollection>> ListMappedPointsFeatureCollection()
    {
        var projectsThatShouldShowOnMap = DbContext.Projects
            .AsNoTracking()
            .Include(x => x.ProjectOrganizations)
            .Include(x => x.ProjectPrograms)
            .Include(x => x.ProjectClassifications);

        var featureCollection = new FeatureCollection();
        var mappedPointFeatures = await projectsThatShouldShowOnMap
            .Where(x => x.ProjectLocationPoint != null)
            .Select(x =>
                new Feature(x.ProjectLocationPoint, new AttributesTable
                {
                    { "ProjectID", x.ProjectID },
                    { "ProjectStageID", x.ProjectStageID },
                    { "ProjectTypeID", x.ProjectTypeID},
                    { "OrganizationID", x.ProjectOrganizations
                        .Where(po => po.RelationshipType.IsPrimaryContact)
                        .Select(po => po.Organization.OrganizationID)
                        .SingleOrDefault() },
                    { "ProgramID", string.Join(",", x.ProjectPrograms.Select(y => y.ProgramID)) },
                    { "ClassificationID", string.Join(",", x.ProjectClassifications.Select(y => y.ClassificationID)) },

                })
            ).ToListAsync();
        foreach (var feature in mappedPointFeatures)
        {
            featureCollection.Add(feature);
        }

        return Ok(featureCollection);
    }

    [HttpGet("no-simple-location")]
    public async Task<ActionResult<IEnumerable<ProjectSimpleTree>>> ListProjectsWithNoSimpleLocation()
    {
        var projects = await Projects.ListWithNoSimpleLocationAsProjectSimpleTree(DbContext);
        return Ok(projects);
    }
}
