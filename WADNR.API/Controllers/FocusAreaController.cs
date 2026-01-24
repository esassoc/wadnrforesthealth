using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.FocusArea;

namespace WADNR.API.Controllers;

[ApiController]
[Route("focus-areas")]
public class FocusAreaController(
    WADNRDbContext dbContext,
    ILogger<FocusAreaController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FocusAreaController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<FocusAreaGridRow>>> List()
    {
        var focusAreas = await FocusAreas.ListAsGridRowAsync(DbContext);
        return Ok(focusAreas);
    }

    [HttpGet("{focusAreaID}")]
    [AllowAnonymous]
    public async Task<ActionResult<FocusAreaDetail>> GetByID([FromRoute] int focusAreaID)
    {
        var focusArea = await FocusAreas.GetByIDAsDetailAsync(DbContext, focusAreaID);
        if (focusArea == null)
        {
            return NotFound();
        }
        return Ok(focusArea);
    }

    [HttpGet("for-region/{dnrUplandRegionID}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FocusAreaGridRow>>> ListForRegion([FromRoute] int dnrUplandRegionID)
    {
        var focusAreas = await FocusAreas.ListForRegionAsGridRowAsync(DbContext, dnrUplandRegionID);
        return Ok(focusAreas);
    }
}
