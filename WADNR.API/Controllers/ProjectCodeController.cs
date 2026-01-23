using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProjectCode;

namespace WADNR.API.Controllers;

[ApiController]
[Route("project-codes")]
public class ProjectCodeController(
    WADNRDbContext dbContext,
    ILogger<ProjectCodeController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectCodeController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectCodeGridRow>>> List()
    {
        var projectCodes = await ProjectCodes.ListAsGridRowAsync(DbContext);
        return Ok(projectCodes);
    }

    [HttpGet("{projectCodeID}")]
    public async Task<ActionResult<ProjectCodeDetail>> GetByID([FromRoute] int projectCodeID)
    {
        var projectCode = await ProjectCodes.GetByIDAsDetailAsync(DbContext, projectCodeID);
        if (projectCode == null)
        {
            return NotFound();
        }
        return Ok(projectCode);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<ProjectCodeLookupItem>>> ListLookup()
    {
        var projectCodes = await ProjectCodes.ListAsLookupItemAsync(DbContext);
        return Ok(projectCodes);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProjectCodeLookupItem>>> Search([FromQuery] string? term)
    {
        var projectCodes = await ProjectCodes.SearchAsLookupItemAsync(DbContext, term ?? string.Empty);
        return Ok(projectCodes);
    }
}
