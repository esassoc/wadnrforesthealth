using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("external-map-layers")]
public class ExternalMapLayerController(
    WADNRDbContext dbContext,
    ILogger<ExternalMapLayerController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ExternalMapLayerController>(dbContext, logger, configuration)
{
    [HttpGet("project-map")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ExternalMapLayerDetail>>> ListForProjectMap()
    {
        var items = await ExternalMapLayers.ListForProjectMapAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("priority-landscape")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ExternalMapLayerDetail>>> ListForPriorityLandscape()
    {
        var items = await ExternalMapLayers.ListForPriorityLandscapeAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("other-maps")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ExternalMapLayerDetail>>> ListForOtherMaps()
    {
        var items = await ExternalMapLayers.ListForOtherMapsAsync(DbContext);
        return Ok(items);
    }
}
