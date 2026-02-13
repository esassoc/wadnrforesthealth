using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using NetTopologySuite.Features;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Controllers;

[ApiController]
[Route("gis-bulk-import")]
public class GisBulkImportController(
    WADNRDbContext dbContext,
    ILogger<GisBulkImportController> logger,
    IOptions<WADNRConfiguration> configuration,
    GDALAPIService gdalApiService = null)
    : SitkaController<GisBulkImportController>(dbContext, logger, configuration)
{
    [HttpGet("source-organizations")]
    [AdminFeature]
    public async Task<ActionResult<List<GisUploadSourceOrganizationSummary>>> ListSourceOrganizations()
    {
        var sourceOrgs = await GisBulkImports.ListSourceOrganizationsAsync(DbContext);
        return Ok(sourceOrgs);
    }

    [HttpPost("attempts")]
    [AdminFeature]
    public async Task<ActionResult<GisUploadAttemptDetail>> CreateAttempt([FromBody] GisUploadAttemptCreateRequest request)
    {
        var detail = await GisBulkImports.CreateAttemptAsync(DbContext, request.GisUploadSourceOrganizationID, CallingUser.PersonID);
        return CreatedAtAction(nameof(GetAttempt), new { gisUploadAttemptID = detail.GisUploadAttemptID }, detail);
    }

    [HttpGet("attempts/{gisUploadAttemptID}")]
    [AdminFeature]
    public async Task<ActionResult<GisUploadAttemptDetail>> GetAttempt([FromRoute] int gisUploadAttemptID)
    {
        var detail = await GisBulkImports.GetAttemptDetailAsync(DbContext, gisUploadAttemptID);
        if (detail == null)
        {
            return NotFound();
        }
        return Ok(detail);
    }

    [HttpPost("attempts/{gisUploadAttemptID}/upload")]
    [AdminFeature]
    [RequestSizeLimit(500_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
    public async Task<ActionResult<GisUploadAttemptDetail>> UploadFile([FromRoute] int gisUploadAttemptID, IFormFile file)
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
            return BadRequest(new { ErrorMessage = "File must be a .zip archive." });
        }

        // Determine file type and get feature classes
        List<GdbFeatureClassPreview> featureClasses;
        try
        {
            featureClasses = await gdalApiService.OgrInfoGdbToFeatureClassInfo(file);
        }
        catch
        {
            featureClasses = new List<GdbFeatureClassPreview>();
        }

        if (featureClasses.Count == 0)
        {
            // Try shapefile format
            featureClasses = await gdalApiService.OgrInfoShpToFeatureClassInfo(file);
        }

        if (featureClasses.Count == 0)
        {
            return BadRequest(new { ErrorMessage = "No feature classes found in the uploaded file." });
        }

        // Convert first feature class to GeoJSON and process
        var firstLayer = featureClasses[0];
        string geoJson;
        try
        {
            geoJson = await gdalApiService.Ogr2OgrGdbLayerToGeoJson(file, firstLayer.FeatureClassName);
        }
        catch
        {
            // Try shapefile format
            geoJson = await gdalApiService.Ogr2OgrShpLayerToGeoJson(file, firstLayer.FeatureClassName);
        }

        await GisBulkImports.UploadAndProcessFileAsync(DbContext, gisUploadAttemptID, geoJson);

        var detail = await GisBulkImports.GetAttemptDetailAsync(DbContext, gisUploadAttemptID);
        return Ok(detail);
    }

    [HttpGet("attempts/{gisUploadAttemptID}/features")]
    [AdminFeature]
    public async Task<ActionResult<List<GisFeatureGridRow>>> GetFeatures([FromRoute] int gisUploadAttemptID)
    {
        var features = await GisBulkImports.GetFeaturesAsGridRowAsync(DbContext, gisUploadAttemptID);
        return Ok(features);
    }

    [HttpGet("attempts/{gisUploadAttemptID}/features-geojson")]
    [AdminFeature]
    public async Task<ActionResult<FeatureCollection>> GetFeaturesGeoJson([FromRoute] int gisUploadAttemptID)
    {
        var featureCollection = await GisBulkImports.GetFeaturesAsFeatureCollectionAsync(DbContext, gisUploadAttemptID);
        return Ok(featureCollection);
    }

    [HttpGet("attempts/{gisUploadAttemptID}/metadata-attributes")]
    [AdminFeature]
    public async Task<ActionResult<List<GisMetadataAttributeItem>>> GetMetadataAttributes([FromRoute] int gisUploadAttemptID)
    {
        var attributes = await GisBulkImports.GetMetadataAttributesAsync(DbContext, gisUploadAttemptID);
        return Ok(attributes);
    }

    [HttpGet("attempts/{gisUploadAttemptID}/default-mappings")]
    [AdminFeature]
    public async Task<ActionResult<GisMetadataMappingDefaults>> GetDefaultMappings([FromRoute] int gisUploadAttemptID)
    {
        var defaults = await GisBulkImports.GetDefaultMappingsAsync(DbContext, gisUploadAttemptID);
        return Ok(defaults);
    }

    [HttpPost("attempts/{gisUploadAttemptID}/import")]
    [AdminFeature]
    public async Task<ActionResult<GisBulkImportResult>> ImportProjects([FromRoute] int gisUploadAttemptID, [FromBody] GisBulkImportRequest request)
    {
        var result = await GisBulkImports.ImportProjectsAsync(DbContext, gisUploadAttemptID, request);
        return Ok(result);
    }
}
