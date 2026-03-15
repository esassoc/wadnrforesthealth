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
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [ProgramManageFeature]
    public async Task<ActionResult<ProgramDetail>> Create([FromBody] ProgramUpsertRequest dto)
    {
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
        var updated = await Programs.UpdateAsync(DbContext, programID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{programID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<IActionResult> Delete([FromRoute] int programID)
    {
        var deleted = await Programs.DeleteAsync(DbContext, programID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
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
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{programID}/notifications/{notificationConfigID}")]
    [ProgramManageFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<IActionResult> DeleteNotification([FromRoute] int programID, [FromRoute] int notificationConfigID)
    {
        var deleted = await Programs.DeleteNotificationAsync(DbContext, notificationConfigID);
        if (!deleted) return NotFound();
        return NoContent();
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

        var projects = await DbContext.ProjectPrograms
            .AsNoTracking()
            .Where(pp => pp.ProgramID == programID && pp.Project.ProjectLocationPoint != null)
            .Select(pp => new
            {
                pp.Project.ProjectID,
                pp.Project.ProjectName,
                pp.Project.ProjectStageID,
                pp.Project.ProjectTypeID,
                TaxonomyBranchID = pp.Project.ProjectType.TaxonomyBranchID,
                TaxonomyTrunkID = pp.Project.ProjectType.TaxonomyBranch.TaxonomyTrunkID,
                Longitude = pp.Project.ProjectLocationPoint.Coordinate.X,
                Latitude = pp.Project.ProjectLocationPoint.Coordinate.Y,
                ClassificationIDs = pp.Project.ProjectClassifications
                    .Select(pc => pc.ClassificationID).ToList(),
                ProgramIDs = pp.Project.ProjectPrograms
                    .Select(prp => prp.ProgramID).ToList(),
                Organizations = pp.Project.ProjectOrganizations
                    .Select(po => new { po.OrganizationID, po.RelationshipTypeID, po.RelationshipType.RelationshipTypeName, po.RelationshipType.IsPrimaryContact }).ToList(),
            })
            .ToListAsync();

        if (projects.Count == 0)
        {
            return BadRequest("No projects with location data found for this program.");
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var features = projects.Select(p =>
        {
            var properties = new Dictionary<string, object>
            {
                ["TaxonomyTrunkID"] = p.TaxonomyTrunkID,
                ["ProjectStageID"] = p.ProjectStageID,
                ["ProjectStageColor"] = ProjectStage.AllLookupDictionary.TryGetValue(p.ProjectStageID, out var stage) ? stage.ProjectStageColor : "",
                ["Info"] = p.ProjectName,
                ["ProjectID"] = p.ProjectID,
                ["TaxonomyBranchID"] = p.TaxonomyBranchID,
                ["ProjectTypeID"] = p.ProjectTypeID,
                ["ClassificationID"] = string.Join(",", p.ClassificationIDs),
            };

            foreach (var group in p.Organizations.GroupBy(o => o.RelationshipTypeName))
            {
                properties[$"{group.Key}ID"] = group.Select(o => o.OrganizationID).ToList();
            }

            properties["PopupUrl"] = $"{baseUrl}/api/projects/{p.ProjectID}/map-popup-html";
            properties["ProgramID"] = string.Join(",", p.ProgramIDs);
            properties["LeadImplementerID"] = (object)(p.Organizations.FirstOrDefault(o => o.IsPrimaryContact)?.OrganizationID ?? -1);
            properties["FeatureColor"] = "#99b3ff";

            return new
            {
                type = "Feature",
                geometry = new
                {
                    type = "Point",
                    coordinates = new[] { p.Longitude, p.Latitude }
                },
                properties
            };
        });

        var featureCollection = new { type = "FeatureCollection", features };
        var geoJson = JsonSerializer.Serialize(featureCollection);

        var programDisplayName = program.IsDefaultProgramForImportOnly
            ? $"{program.Organization.OrganizationName} ({program.Organization.OrganizationShortName})"
            : program.ProgramName;
        var dateStr = DateTime.Now.ToString("yyyy-MM-dd");
        var gdbName = $"ProjectsInProgram-{programDisplayName}-{dateStr}";
        var fileName = $"{gdbName}.gdb.zip";

        var stream = await gdalApiService.Ogr2OgrGeoJsonToGdb(geoJson, "ProjectLocationSimple", gdbName);
        return File(stream, "application/zip", fileName);
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
    public async Task<ActionResult> DeleteBlockListEntry([FromRoute] int programID, [FromRoute] int projectImportBlockListID)
    {
        var deleted = await Programs.DeleteBlockListEntryAsync(DbContext, projectImportBlockListID);
        if (!deleted) return NotFound();
        return NoContent();
    }

    #endregion

    [HttpPut("{programID}/gis-import-config/basics")]
    [ProgramEditMappingsFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<GdbImportBasics>> UpdateGdbImportBasics([FromRoute] int programID, [FromBody] GdbImportBasicsUpsertRequest request)
    {
        var result = await Programs.UpdateGdbImportBasicsAsync(DbContext, programID, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{programID}/gis-import-config/default-mappings")]
    [ProgramEditMappingsFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<List<GdbDefaultMappingItem>>> UpdateGdbDefaultMappings([FromRoute] int programID, [FromBody] GdbDefaultMappingUpsertRequest request)
    {
        var result = await Programs.UpdateGdbDefaultMappingsAsync(DbContext, programID, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{programID}/gis-import-config/crosswalk-values")]
    [ProgramEditMappingsFeature]
    [EntityNotFound(typeof(WADNR.EFModels.Entities.Program), "programID")]
    public async Task<ActionResult<List<GdbCrosswalkItem>>> UpdateGdbCrosswalkValues([FromRoute] int programID, [FromBody] GdbCrosswalkUpsertRequest request)
    {
        var result = await Programs.UpdateGdbCrosswalkValuesAsync(DbContext, programID, request);
        if (result == null) return NotFound();
        return Ok(result);
    }
}