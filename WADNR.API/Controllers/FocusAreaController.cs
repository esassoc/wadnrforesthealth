using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
using WADNR.Models.DataTransferObjects.Shared;

namespace WADNR.API.Controllers;

[ApiController]
[Route("focus-areas")]
public class FocusAreaController(
    WADNRDbContext dbContext,
    ILogger<FocusAreaController> logger,
    IOptions<WADNRConfiguration> configuration,
    GDALAPIService gdalApiService = null)
    : SitkaController<FocusAreaController>(dbContext, logger, configuration)
{
    [HttpGet]
    [NormalUserFeature]
    public async Task<ActionResult<List<FocusAreaGridRow>>> List()
    {
        var focusAreas = await FocusAreas.ListAsGridRowAsync(DbContext);
        return Ok(focusAreas);
    }

    [HttpGet("locations")]
    [NormalUserFeature]
    public async Task<ActionResult<FeatureCollection>> ListLocations()
    {
        var features = await FocusAreas.ListLocationsAsFeatureCollectionAsync(DbContext);
        return Ok(features);
    }

    [HttpGet("{focusAreaID}")]
    [NormalUserFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<ActionResult<FocusAreaDetail>> GetByID([FromRoute] int focusAreaID)
    {
        var focusArea = await FocusAreas.GetByIDAsDetailAsync(DbContext, focusAreaID);
        if (focusArea == null)
        {
            return NotFound();
        }
        return Ok(focusArea);
    }

    [HttpPost]
    [FocusAreaManageFeature]
    public async Task<ActionResult<FocusAreaDetail>> Create([FromBody] FocusAreaUpsertRequest dto)
    {
        var created = await FocusAreas.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(GetByID), new { focusAreaID = created.FocusAreaID }, created);
    }

    [HttpPut("{focusAreaID}")]
    [FocusAreaManageFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<ActionResult<FocusAreaDetail>> Update([FromRoute] int focusAreaID, [FromBody] FocusAreaUpsertRequest dto)
    {
        var updated = await FocusAreas.UpdateAsync(DbContext, focusAreaID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{focusAreaID}")]
    [FocusAreaManageFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<IActionResult> Delete([FromRoute] int focusAreaID)
    {
        var (success, errorMessage) = await FocusAreas.DeleteAsync(DbContext, focusAreaID);
        if (!success)
        {
            return BadRequest(new { ErrorMessage = errorMessage });
        }
        return NoContent();
    }

    [HttpGet("{focusAreaID}/location")]
    [NormalUserFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<ActionResult<FeatureCollection>> GetLocation([FromRoute] int focusAreaID)
    {
        var features = await FocusAreas.GetLocationAsFeatureCollectionAsync(DbContext, focusAreaID);
        return Ok(features);
    }

    [HttpPost("{focusAreaID}/location/upload-gdb")]
    [FocusAreaManageFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    [RequestSizeLimit(500_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
    public async Task<ActionResult<List<GdbFeatureClassPreview>>> UploadGdbForLocation([FromRoute] int focusAreaID, IFormFile file)
    {
        if (gdalApiService == null)
        {
            return StatusCode(503, new { ErrorMessage = "GDB import is not configured on this server." });
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { ErrorMessage = "A file is required." });
        }

        if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { ErrorMessage = "File must be a .zip archive containing a File Geodatabase (.gdb)." });
        }

        var featureClasses = await gdalApiService.OgrInfoGdbToFeatureClassInfo(file);

        await FocusAreas.ClearAndSaveStagingAsync(DbContext, focusAreaID, featureClasses,
            featureClassName => gdalApiService.Ogr2OgrGdbLayerToGeoJson(file, featureClassName));

        return Ok(featureClasses);
    }

    [HttpGet("{focusAreaID}/location/staged-features")]
    [FocusAreaManageFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<ActionResult<List<StagedFeatureLayer>>> GetStagedFeatures([FromRoute] int focusAreaID)
    {
        var features = await FocusAreas.GetStagedFeaturesAsync(DbContext, focusAreaID);
        return Ok(features);
    }

    [HttpPost("{focusAreaID}/location/approve-gdb")]
    [FocusAreaManageFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<IActionResult> ApproveGdbForLocation([FromRoute] int focusAreaID, [FromBody] SinglePolygonApproveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SelectedGeometryWkt))
        {
            return BadRequest(new { ErrorMessage = "A geometry selection is required." });
        }

        var success = await FocusAreas.ApproveSinglePolygonAsync(DbContext, focusAreaID, request.SelectedGeometryWkt);
        if (!success)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpGet("{focusAreaID}/projects")]
    [NormalUserFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<ActionResult<List<ProjectFocusAreaDetailGridRow>>> ListProjects([FromRoute] int focusAreaID)
    {
        var projects = await Projects.ListForFocusAreaAsGridRowAsync(DbContext, focusAreaID);
        return Ok(projects);
    }

    [HttpGet("{focusAreaID}/projects/feature-collection")]
    [NormalUserFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<ActionResult<FeatureCollection>> ListProjectsFeatureCollection([FromRoute] int focusAreaID)
    {
        var projectQuery = DbContext.Projects.Where(p => p.FocusAreaID == focusAreaID);
        var featureCollection = await Projects.MapProjectFeatureCollection(projectQuery);
        return Ok(featureCollection);
    }

    [HttpDelete("{focusAreaID}/location")]
    [FocusAreaManageFeature]
    [EntityNotFound(typeof(FocusArea), "focusAreaID")]
    public async Task<IActionResult> DeleteLocation([FromRoute] int focusAreaID)
    {
        var deleted = await FocusAreas.DeleteLocationAsync(DbContext, focusAreaID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
