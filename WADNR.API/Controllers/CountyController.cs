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

namespace WADNR.API.Controllers;

[ApiController]
[Route("counties")]
public class CountyController(
    WADNRDbContext dbContext,
    ILogger<CountyController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<CountyController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CountyGridRow>>> List()
    {
        var items = await Counties.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{countyID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(County), "countyID")]
    public async Task<ActionResult<CountyDetail>> Get([FromRoute] int countyID)
    {
        var entity = await Counties.GetByIDAsDetailAsync(DbContext, countyID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpGet("{countyID}/projects")]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<ProjectCountyDetailGridRow>>> ListProjectsForCountyID([FromRoute] int countyID)
    {
        var projects = await Projects.ListAsCountyDetailGridRowForUserAsync(DbContext, countyID, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{countyID}/projects/feature-collection")]
    [AllowAnonymous]
    [EntityNotFound(typeof(County), "countyID")]
    public async Task<ActionResult<FeatureCollection>> ListProjectsFeatureCollectionForCountyID([FromRoute] int countyID)
    {
        var projectQuery = DbContext.ProjectCounties
            .Where(pc => pc.CountyID == countyID)
            .Select(pc => pc.Project);
        var featureCollection = await Projects.MapProjectFeatureCollection(projectQuery);
        return Ok(featureCollection);
    }
}
