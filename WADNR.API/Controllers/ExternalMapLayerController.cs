using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
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

    [HttpGet]
    [AdminFeature]
    public async Task<ActionResult<List<ExternalMapLayerDetail>>> List()
    {
        var items = await ExternalMapLayers.ListAllAsDetailAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{externalMapLayerID:int}")]
    [AdminFeature]
    public async Task<ActionResult<ExternalMapLayerDetail>> GetByID([FromRoute] int externalMapLayerID)
    {
        var entity = await DbContext.ExternalMapLayers
            .AsNoTracking()
            .Where(x => x.ExternalMapLayerID == externalMapLayerID)
            .Select(ExternalMapLayerProjections.AsDetail)
            .SingleOrDefaultAsync();
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [AdminFeature]
    public async Task<ActionResult<ExternalMapLayerDetail>> Create([FromBody] ExternalMapLayerUpsertRequest request)
    {
        var validationError = await ValidateUpsertRequest(request, null);
        if (validationError != null)
        {
            return BadRequest(validationError);
        }

        var created = await ExternalMapLayers.CreateAsync(DbContext, request);
        return Ok(created);
    }

    [HttpPut("{externalMapLayerID:int}")]
    [AdminFeature]
    public async Task<ActionResult<ExternalMapLayerDetail>> Update([FromRoute] int externalMapLayerID, [FromBody] ExternalMapLayerUpsertRequest request)
    {
        var entity = await ExternalMapLayers.GetByIDAsync(DbContext, externalMapLayerID);
        if (entity == null)
        {
            return NotFound();
        }

        var validationError = await ValidateUpsertRequest(request, externalMapLayerID);
        if (validationError != null)
        {
            return BadRequest(validationError);
        }

        var updated = await ExternalMapLayers.UpdateAsync(DbContext, entity, request);
        return Ok(updated);
    }

    [HttpDelete("{externalMapLayerID:int}")]
    [AdminFeature]
    public async Task<IActionResult> Delete([FromRoute] int externalMapLayerID)
    {
        var entity = await ExternalMapLayers.GetByIDAsync(DbContext, externalMapLayerID);
        if (entity == null)
        {
            return NotFound();
        }

        await ExternalMapLayers.DeleteAsync(DbContext, entity);
        return NoContent();
    }

    private async Task<string?> ValidateUpsertRequest(ExternalMapLayerUpsertRequest request, int? excludeID)
    {
        var duplicateName = await DbContext.ExternalMapLayers
            .AnyAsync(x => x.DisplayName == request.DisplayName && (excludeID == null || x.ExternalMapLayerID != excludeID));
        if (duplicateName)
        {
            return "A map layer with this display name already exists.";
        }

        var duplicateUrl = await DbContext.ExternalMapLayers
            .AnyAsync(x => x.LayerUrl == request.LayerUrl && (excludeID == null || x.ExternalMapLayerID != excludeID));
        if (duplicateUrl)
        {
            return "A map layer with this URL already exists.";
        }

        if (request.IsTiledMapService && !string.IsNullOrWhiteSpace(request.FeatureNameField))
        {
            return "Tiled map services cannot have a Feature Name Field.";
        }

        return null;
    }
}
