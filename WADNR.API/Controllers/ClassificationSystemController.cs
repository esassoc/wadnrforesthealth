using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ClassificationSystem;

namespace WADNR.API.Controllers;

[ApiController]
[Route("classification-systems")]
public class ClassificationSystemController(
    WADNRDbContext dbContext,
    ILogger<ClassificationSystemController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ClassificationSystemController>(dbContext, logger, configuration)
{
    [HttpGet]
    public async Task<ActionResult<List<ClassificationSystemGridRow>>> List()
    {
        var classificationSystems = await ClassificationSystems.ListAsGridRowAsync(DbContext);
        return Ok(classificationSystems);
    }

    [HttpGet("{classificationSystemID}")]
    public async Task<ActionResult<ClassificationSystemDetail>> GetByID([FromRoute] int classificationSystemID)
    {
        var classificationSystem = await ClassificationSystems.GetByIDAsDetailAsync(DbContext, classificationSystemID);
        if (classificationSystem == null)
        {
            return NotFound();
        }
        return Ok(classificationSystem);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<ClassificationSystemLookupItem>>> ListLookup()
    {
        var classificationSystems = await ClassificationSystems.ListAsLookupItemAsync(DbContext);
        return Ok(classificationSystems);
    }
}
