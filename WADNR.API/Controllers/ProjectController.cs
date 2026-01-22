using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController(
    WADNRDbContext dbContext,
    ILogger<ProjectController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> List()
    {
        var projects = await Projects.ListAsGridRowAsync(DbContext);
        return Ok(projects);
    }

    [HttpGet("{projectID}")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectDetail>> Get([FromRoute] int projectID)
    {
        var entity = await Projects.GetByIDAsDetailAsync(DbContext, projectID);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpGet("{projectID}/fact-sheet")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectFactSheet>> GetForFactSheet([FromRoute] int projectID)
    {
        var entity = await Projects.GetByIDAsFactSheetAsync(DbContext, projectID);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpGet("{projectID}/map-popup")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectMapPopup>> GetAsMapPopup([FromRoute] int projectID)
    {
        var entity = await Projects.GetByIDAsMapPopupAsync(DbContext, projectID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    //[AdminFeature]
    public async Task<ActionResult<ProjectDetail>> Create([FromBody] ProjectUpsertRequest dto)
    {
        var created = await Projects.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { projectID = created.ProjectID }, created);
    }

    [HttpPut("{projectID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectDetail>> Update([FromRoute] int projectID, [FromBody] ProjectUpsertRequest dto)
    {
        var updated = await Projects.UpdateAsync(DbContext, projectID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{projectID}")]
    //[AdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> Delete([FromRoute] int projectID)
    {
        var deleted = await Projects.DeleteAsync(DbContext, projectID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("mapped-point/feature-collection")]
    public async Task<ActionResult<FeatureCollection>> ListMappedPointsFeatureCollection()
    {
        var projectsThatShouldShowOnMap = DbContext.Projects
            .AsNoTracking()
            .Include(x => x.ProjectOrganizations)
            .Include(x => x.ProjectPrograms)
            .Include(x => x.ProjectClassifications);

        var featureCollection = await Projects.MapProjectFeatureCollection(projectsThatShouldShowOnMap);

        return Ok(featureCollection);
    }

    [HttpGet("no-simple-location")]
    public async Task<ActionResult<IEnumerable<ProjectSimpleTree>>> ListProjectsWithNoSimpleLocation()
    {
        var projects = await Projects.ListWithNoSimpleLocationAsProjectSimpleTree(DbContext);
        return Ok(projects);
    }

    [HttpGet("{projectID}/images")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectImageGridRow>>> ListImages([FromRoute] int projectID)
    {
        var entities = await ProjectImages.ListAsGridRowAsync(DbContext, projectID);
        return Ok(entities);
    }

    [HttpGet("{projectID}/classifications")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ClassificationLookupItem>>> ListClassifications([FromRoute] int projectID)
    {
        var classifications = await Projects.ListClassificationsAsLookupItemByProjectIDAsync(DbContext, projectID);
        return Ok(classifications);
    }

    [HttpGet("{projectID}/treatments")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<TreatmentGridRow>>> ListTreatments([FromRoute] int projectID)
    {
        var treatments = await Treatments.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(treatments);
    }

    [HttpGet("{projectID}/interaction-events")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<InteractionEventGridRow>>> ListInteractionEvents([FromRoute] int projectID)
    {
        var events = await InteractionEvents.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(events);
    }

    [HttpGet("{projectID}/locations/generic-layers")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<List<GenericLayer>>> ListLocationsAsGenericLayers([FromRoute] int projectID)
    {
        var project = await DbContext.Projects.AsNoTracking().Include(x => x.ProjectLocations)
            .FirstOrDefaultAsync(x => x.ProjectID == projectID);
        var genericLayers = new List<GenericLayer>();
        if (project.ProjectLocationPoint != null)
        {
            genericLayers.Add(new GenericLayer
            {
                LayerName = "Project Location - Simple",
                LayerColor = "#fcfc12",
                Features = new FeatureCollection { new Feature(project.ProjectLocationPoint, new AttributesTable()) }
            });
        }

        if (project.ProjectLocations == null || project.ProjectLocations.Count == 0)
        {
            return Ok();
        }

        var projectLocationTypes = project.ProjectLocations.Select(x => x.ProjectLocationType).Distinct().ToList();

        foreach (var projectLocationType in projectLocationTypes)
        {
            var featureCollection = new FeatureCollection();
            foreach (var location in project.ProjectLocations.Where(x => x.ProjectLocationType == projectLocationType))
            {
                featureCollection.Add(new Feature(location.ProjectLocationGeometry, new AttributesTable {}));
            }

            genericLayers.Add(new GenericLayer
            {
                LayerName = $"Project Location - Detail - {projectLocationType.ProjectLocationTypeDisplayName}",
                LayerColor = projectLocationType.ProjectLocationTypeMapLayerColor,
                Features = featureCollection
            });
        }

        return Ok(genericLayers);
    }

    [HttpGet("{projectID}/documents")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectDocumentGridRow>>> ListDocuments([FromRoute] int projectID)
    {
        var documents = await ProjectDocuments.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(documents);
    }

    [HttpGet("{projectID}/notes")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectNoteGridRow>>> ListNotes([FromRoute] int projectID)
    {
        var notes = await ProjectNotes.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(notes);
    }

    [HttpGet("{projectID}/external-links")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectExternalLinkGridRow>>> ListExternalLinks([FromRoute] int projectID)
    {
        var links = await ProjectExternalLinks.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(links);
    }

    [HttpGet("{projectID}/update-history")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectUpdateHistoryGridRow>>> ListUpdateHistory([FromRoute] int projectID)
    {
        var updates = await ProjectUpdateBatches.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(updates);
    }

    [HttpGet("{projectID}/notifications")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectNotificationGridRow>>> ListNotifications([FromRoute] int projectID)
    {
        var notifications = await Notifications.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(notifications);
    }

    [HttpGet("{projectID}/audit-logs")]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectAuditLogGridRow>>> ListAuditLogs([FromRoute] int projectID)
    {
        var logs = await AuditLogs.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(logs);
    }
}
