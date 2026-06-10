using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.Common.GeoSpatial;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("programs")]
public class ProgramController(
    WADNRDbContext dbContext,
    ILogger<ProgramController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService,
    GDALAPIService gdalApiService = null)
    : SitkaController<ProgramController>(dbContext, logger, configuration)
{
    [HttpGet]
    [ProgramViewFeature]
    public async Task<ActionResult<IEnumerable<ProgramGridRow>>> List()
    {
        var sources = await Programs.ListAsGridRowAsync(DbContext);
        return Ok(sources);
    }

    [HttpGet("{programID}")]
    [ProgramViewFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<ProgramDetail>> Get([FromRoute] int programID)
    {
        var entity = await Programs.GetByIDAsDetailAsync(DbContext, programID);
        return RequireNotNullThrowNotFound(entity, "Program", programID);
    }

    [HttpPost]
    [ProgramManageFeature]
    public async Task<ActionResult<ProgramDetail>> Create([FromBody] ProgramUpsertRequest dto)
    {
        var validationError = await Programs.ValidateUpsertAsync(DbContext, dto);
        if (validationError != null) return BadRequest(new { message = validationError });

        var created = await Programs.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { programID = created.ProgramID }, created);
    }

    [HttpPut("{programID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<ProgramDetail>> Update([FromRoute] int programID, [FromBody] ProgramUpsertRequest dto)
    {
        var validationError = await Programs.ValidateUpsertAsync(DbContext, dto, programID);
        if (validationError != null) return BadRequest(new { message = validationError });

        var updated = await Programs.UpdateAsync(DbContext, programID, dto, CallingUser.PersonID);
        return RequireNotNullThrowNotFound(updated, "Program", programID);
    }

    [HttpGet("{programID}/delete-info")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<ProgramDeleteInfo>> GetDeleteInfo([FromRoute] int programID)
    {
        var info = await Programs.GetDeleteInfoAsync(DbContext, programID);
        return RequireNotNullThrowNotFound(info, "Program", programID);
    }

    [HttpDelete("{programID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<IActionResult> Delete([FromRoute] int programID)
    {
        var deleted = await Programs.DeleteAsync(DbContext, programID);
        return DeleteOrNotFound(deleted);
    }

    [HttpGet("eligible-editors")]
    [ProgramManageFeature]
    public async Task<ActionResult<List<PersonLookupItem>>> ListEligibleEditors()
    {
        var editors = await Programs.ListEligibleProgramEditorsAsync(DbContext);
        return Ok(editors);
    }

    [HttpPut("{programID}/editors")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<List<PersonWithOrganizationLookupItem>>> UpdateEditors([FromRoute] int programID, [FromBody] ProgramEditorsUpsertRequest request)
    {
        if (request.PersonIDList.Count > 0)
        {
            var validationError = await Programs.ValidateEditorsHaveRequiredRoleAsync(DbContext, request.PersonIDList);
            if (validationError != null)
                return BadRequest(validationError);
        }

        var updatedEditors = await Programs.UpdateEditorsAsync(DbContext, programID, request);
        return Ok(updatedEditors);
    }

    [HttpGet("{programID}/projects")]
    [ProgramViewFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<IEnumerable<ProjectProgramDetailGridRow>>> ListProjects([FromRoute] int programID)
    {
        var projects = await Programs.ListProjectsForProgramAsync(DbContext, programID);
        return Ok(projects);
    }

    [HttpGet("{programID}/notifications")]
    [ProgramViewFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<IEnumerable<ProgramNotificationGridRow>>> ListNotifications([FromRoute] int programID)
    {
        var notifications = await Programs.ListNotificationsForProgramAsync(DbContext, programID);
        return Ok(notifications);
    }

    [HttpPost("{programID}/notifications")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<ProgramNotificationGridRow>> CreateNotification([FromRoute] int programID, [FromBody] ProgramNotificationUpsertRequest request)
    {
        var created = await Programs.CreateNotificationAsync(DbContext, programID, request);
        return Ok(created);
    }

    [HttpPut("{programID}/notifications/{notificationConfigID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<ProgramNotificationGridRow>> UpdateNotification([FromRoute] int programID, [FromRoute] int notificationConfigID, [FromBody] ProgramNotificationUpsertRequest request)
    {
        var updated = await Programs.UpdateNotificationAsync(DbContext, notificationConfigID, request);
        return RequireNotNullThrowNotFound(updated, "ProgramNotificationConfiguration", notificationConfigID);
    }

    [HttpDelete("{programID}/notifications/{notificationConfigID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<IActionResult> DeleteNotification([FromRoute] int programID, [FromRoute] int notificationConfigID)
    {
        var deleted = await Programs.DeleteNotificationAsync(DbContext, notificationConfigID);
        return DeleteOrNotFound(deleted);
    }

    [HttpPost("upload-program-file")]
    [ProgramManageFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> UploadProgramFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);
        return Ok(fileResource.FileResourceID);
    }

    [HttpPost("upload-example-geospatial-file")]
    [ProgramManageFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> UploadExampleGeospatialFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);
        return Ok(fileResource.FileResourceID);
    }

    #region Download GDB

    [HttpGet("{programID}/projects/download-gdb")]
    [ProgramEditMappingsFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<IActionResult> DownloadProjectsAsGdb([FromRoute] int programID)
    {
        if (gdalApiService == null)
        {
            return StatusCode(503, "GDAL API service is not configured.");
        }

        var program = await DbContext.Programs.AsNoTracking()
            .Include(p => p.Organization)
            .FirstOrDefaultAsync(p => p.ProgramID == programID);
        if (program == null) return NotFound();

        var exportData = await Programs.GetGdbExportDataAsync(DbContext, programID);

        if (exportData.ProjectPoints.Count == 0
            && exportData.ProjectLocations.Count == 0
            && exportData.Treatments.Count == 0)
        {
            return BadRequest("No projects with location data found for this program.");
        }

        var layers = new List<(string LayerName, string GeoJson)>();
        if (exportData.ProjectPoints.Count > 0)
        {
            var gisIdentifierLabel = await GetProjectIdentifierLabelAsync();
            layers.Add(("ProjectPoints", SerializeProjectPointsAsGeoJson(exportData.ProjectPoints, gisIdentifierLabel)));
        }
        if (exportData.ProjectLocations.Count > 0)
        {
            layers.Add(("ProjectLocations", SerializeAsGeoJson(exportData.ProjectLocations)));
        }
        if (exportData.Treatments.Count > 0)
        {
            layers.Add(("Treatments", SerializeAsGeoJson(exportData.Treatments)));
        }

        var programDisplayName = program.IsDefaultProgramForImportOnly
            ? $"{program.Organization.OrganizationName} ({program.Organization.OrganizationShortName})"
            : program.ProgramName;
        var dateStr = DateTime.Now.ToString("yyyy-MM-dd");
        var gdbName = $"ProjectsInProgram-{programDisplayName}-{dateStr}";
        var fileName = $"{gdbName}.gdb.zip";

        var stream = await gdalApiService.Ogr2OgrGeoJsonToGdbMultiLayer(layers, gdbName);
        return File(stream, "application/zip", fileName);
    }

    private static string SerializeAsGeoJson<T>(IEnumerable<T> features) where T : IHasGeometry
    {
        var featureCollection = features.Cast<IHasGeometry>().ToFeatureCollection();
        return GeoJsonSerializer.Serialize(featureCollection);
    }

    private string SerializeProjectPointsAsGeoJson(IReadOnlyList<ProgramGdbProjectPointDto> features, string gisIdentifierLabel)
    {
        // Determine the target column name once: if the configured label collides with another
        // attribute we already write (e.g. an admin sets the label to "ProjectName"), keep the
        // original "ProjectGisIdentifier" key rather than silently overwriting the existing
        // attribute. Use any feature's attribute set to check — the schema is identical per row.
        var resolvedLabel = gisIdentifierLabel;
        if (features.Count > 0 && !string.Equals(gisIdentifierLabel, nameof(ProgramGdbProjectPointDto.ProjectGisIdentifier), StringComparison.Ordinal))
        {
            var sampleAttributes = GeoJsonSerializer.ToKeyValuePairList(features[0]);
            sampleAttributes.Remove(nameof(ProgramGdbProjectPointDto.ProjectGisIdentifier));
            if (sampleAttributes.ContainsKey(gisIdentifierLabel))
            {
                Logger.LogWarning(
                    "Configured FieldDefinition label '{Label}' for ProjectIdentifier collides with an existing ProjectPoints column. Falling back to '{Fallback}' for the GDB export.",
                    gisIdentifierLabel, nameof(ProgramGdbProjectPointDto.ProjectGisIdentifier));
                resolvedLabel = nameof(ProgramGdbProjectPointDto.ProjectGisIdentifier);
            }
        }

        var featureCollection = new NetTopologySuite.Features.FeatureCollection();
        foreach (var feature in features)
        {
            var attributes = GeoJsonSerializer.ToKeyValuePairList(feature);
            if (attributes.Remove(nameof(ProgramGdbProjectPointDto.ProjectGisIdentifier), out var gisIdentifierValue))
            {
                attributes[resolvedLabel] = gisIdentifierValue;
            }
            featureCollection.Add(new NetTopologySuite.Features.Feature(feature.Geometry, new NetTopologySuite.Features.AttributesTable(attributes)));
        }
        return GeoJsonSerializer.Serialize(featureCollection);
    }

    private async Task<string> GetProjectIdentifierLabelAsync()
    {
        var customLabel = await DbContext.FieldDefinitionData
            .AsNoTracking()
            .Where(x => x.FieldDefinitionID == (int)FieldDefinitionEnum.ProjectIdentifier)
            .Select(x => x.FieldDefinitionLabel)
            .SingleOrDefaultAsync();

        return !string.IsNullOrWhiteSpace(customLabel)
            ? customLabel
            : FieldDefinition.ProjectIdentifier.FieldDefinitionDisplayName;
    }

    #endregion

    #region Block List

    [HttpGet("{programID}/block-list")]
    [ProgramViewFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<List<ProjectImportBlockListGridRow>>> ListBlockListEntries([FromRoute] int programID)
    {
        var entries = await Programs.ListBlockListEntriesAsync(DbContext, programID);
        return Ok(entries);
    }

    [HttpPost("{programID}/block-list")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult> AddToBlockList([FromRoute] int programID, [FromBody] AddToBlockListRequest request)
    {
        await Programs.AddToBlockListAsync(DbContext, programID, request);
        return Ok();
    }

    [HttpDelete("{programID}/block-list/{projectImportBlockListID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<IActionResult> DeleteBlockListEntry([FromRoute] int programID, [FromRoute] int projectImportBlockListID)
    {
        var deleted = await Programs.DeleteBlockListEntryAsync(DbContext, projectImportBlockListID);
        return DeleteOrNotFound(deleted);
    }

    #endregion

    [HttpPut("{programID}/gis-import-config/basics")]
    [ProgramEditMappingsFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<GdbImportBasics>> UpdateGdbImportBasics([FromRoute] int programID, [FromBody] GdbImportBasicsUpsertRequest request)
    {
        var result = await Programs.UpdateGdbImportBasicsAsync(DbContext, programID, request);
        return RequireNotNullThrowNotFound(result, "Program", programID);
    }

    [HttpPut("{programID}/gis-import-config/default-mappings")]
    [ProgramEditMappingsFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<List<GdbDefaultMappingItem>>> UpdateGdbDefaultMappings([FromRoute] int programID, [FromBody] GdbDefaultMappingUpsertRequest request)
    {
        var result = await Programs.UpdateGdbDefaultMappingsAsync(DbContext, programID, request);
        return RequireNotNullThrowNotFound(result, "Program", programID);
    }

    [HttpPut("{programID}/gis-import-config/crosswalk-values")]
    [ProgramEditMappingsFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<List<GdbCrosswalkItem>>> UpdateGdbCrosswalkValues([FromRoute] int programID, [FromBody] GdbCrosswalkUpsertRequest request)
    {
        var result = await Programs.UpdateGdbCrosswalkValuesAsync(DbContext, programID, request);
        return RequireNotNullThrowNotFound(result, "Program", programID);
    }
}