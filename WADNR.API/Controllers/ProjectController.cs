using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.EFModels.Workflows;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

namespace WADNR.API.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController(
    WADNRDbContext dbContext,
    ILogger<ProjectController> logger,
    IOptions<WADNRConfiguration> configuration,
    ProjectNotificationService notificationService,
    GDALAPIService gdalApiService = null)
    : SitkaController<ProjectController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> List()
    {
        var projects = await Projects.ListAsGridRowForUserAsync(DbContext, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{projectID}")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectDetail>> Get([FromRoute] int projectID)
    {
        var entity = await Projects.GetByIDAsDetailForUserAsync(DbContext, projectID, CallingUser);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpGet("{projectID}/fact-sheet")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectFactSheet>> GetForFactSheet([FromRoute] int projectID)
    {
        var entity = await Projects.GetByIDAsFactSheetForUserAsync(DbContext, projectID, CallingUser);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpGet("{projectID}/map-popup")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectMapPopup>> GetAsMapPopup([FromRoute] int projectID)
    {
        var entity = await Projects.GetByIDAsMapPopupForUserAsync(DbContext, projectID, CallingUser);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [ProjectEditFeature]
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
    [ProjectEditFeature]
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
    [AdminFeature]
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
    [AllowAnonymous]
    [ProjectViewFeature]
    public async Task<ActionResult<FeatureCollection>> ListMappedPointsFeatureCollection()
    {
        var featureCollection = await Projects.MapProjectFeatureCollectionForUser(DbContext, CallingUser);
        return Ok(featureCollection);
    }

    [HttpGet("no-simple-location")]
    [AllowAnonymous]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<ProjectSimpleTree>>> ListProjectsWithNoSimpleLocation()
    {
        var projects = await Projects.ListWithNoSimpleLocationAsProjectSimpleTreeForUserAsync(DbContext, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{projectID}/images")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectImageGridRow>>> ListImages([FromRoute] int projectID)
    {
        var entities = await ProjectImages.ListAsGridRowAsync(DbContext, projectID);
        return Ok(entities);
    }

    [HttpGet("{projectID}/classifications")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ClassificationLookupItem>>> ListClassifications([FromRoute] int projectID)
    {
        var classifications = await Projects.ListClassificationsAsLookupItemByProjectIDForUserAsync(DbContext, projectID, CallingUser);
        return Ok(classifications);
    }

    [HttpGet("{projectID}/treatments")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<TreatmentGridRow>>> ListTreatments([FromRoute] int projectID)
    {
        var treatments = await Treatments.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(treatments);
    }

    [HttpGet("{projectID}/treatment-areas")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<TreatmentAreaLookupItem>>> ListTreatmentAreas([FromRoute] int projectID)
    {
        var treatmentAreas = await Treatments.ListTreatmentAreasForProjectAsync(DbContext, projectID);
        return Ok(treatmentAreas);
    }

    [HttpGet("{projectID}/interaction-events")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<InteractionEventGridRow>>> ListInteractionEvents([FromRoute] int projectID)
    {
        var events = await InteractionEvents.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(events);
    }

    [HttpGet("{projectID}/locations/generic-layers")]
    [AllowAnonymous]
    [ProjectViewFeature]
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
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectDocumentGridRow>>> ListDocuments([FromRoute] int projectID)
    {
        var documents = await ProjectDocuments.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(documents);
    }

    [HttpGet("{projectID}/notes")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectNoteGridRow>>> ListNotes([FromRoute] int projectID)
    {
        var notes = await ProjectNotes.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(notes);
    }

    [HttpGet("{projectID}/internal-notes")]
    [AdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectInternalNoteGridRow>>> ListInternalNotes([FromRoute] int projectID)
    {
        var notes = await ProjectInternalNotes.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(notes);
    }

    [HttpGet("{projectID}/external-links")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectExternalLinkGridRow>>> ListExternalLinks([FromRoute] int projectID)
    {
        var links = await ProjectExternalLinks.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(links);
    }

    [HttpGet("{projectID}/update-history")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectUpdateHistoryGridRow>>> ListUpdateHistory([FromRoute] int projectID)
    {
        var updates = await ProjectUpdateBatches.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(updates);
    }

    [HttpGet("{projectID}/notifications")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectNotificationGridRow>>> ListNotifications([FromRoute] int projectID)
    {
        var notifications = await Notifications.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(notifications);
    }

    [HttpGet("{projectID}/audit-logs")]
    [AdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectAuditLogGridRow>>> ListAuditLogs([FromRoute] int projectID)
    {
        var logs = await AuditLogs.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(logs);
    }

    #region Create Workflow - Progress

    [HttpGet("{projectID}/create-workflow/progress")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<CreateWorkflowProgressResponse>> GetCreateWorkflowProgress([FromRoute] int projectID)
    {
        var progress = await ProjectCreateWorkflowProgress.GetProgressForUserAsync(DbContext, projectID, CallingUser);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    #endregion

    #region Create Workflow Steps - Basics

    [HttpGet("{projectID}/create-workflow/steps/basics")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectBasicsStep>> GetCreateBasicsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetBasicsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/basics")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectBasicsStep>> SaveCreateBasicsStep([FromRoute] int projectID, [FromBody] ProjectBasicsStepRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveBasicsStepAsync(DbContext, projectID, request, CallingUser.PersonID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPost("create-workflow/steps/basics")]
    [ProjectEditFeature]
    public async Task<ActionResult<ProjectBasicsStep>> CreateProjectFromBasicsStep([FromBody] ProjectBasicsStepRequest request)
    {
        var dto = await ProjectWorkflowSteps.CreateProjectFromBasicsStepAsync(DbContext, request, CallingUser.PersonID);
        return CreatedAtAction(nameof(GetCreateBasicsStep), new { projectID = dto.ProjectID }, dto);
    }

    #endregion

    #region Create Workflow Steps - Location Simple

    [HttpGet("{projectID}/create-workflow/steps/location-simple")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationSimpleStep>> GetCreateLocationSimpleStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetLocationSimpleStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/location-simple")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationSimpleStep>> SaveCreateLocationSimpleStep([FromRoute] int projectID, [FromBody] LocationSimpleStepRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveLocationSimpleStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Create Workflow Steps - Location Detailed

    [HttpGet("{projectID}/create-workflow/steps/location-detailed")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStep>> GetCreateLocationDetailedStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetLocationDetailedStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/location-detailed")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStep>> SaveCreateLocationDetailedStep([FromRoute] int projectID, [FromBody] LocationDetailedStepRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveLocationDetailedStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPost("{projectID}/create-workflow/steps/location-detailed/upload-gdb")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    [RequestSizeLimit(500_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
    public async Task<ActionResult<List<GdbFeatureClassPreview>>> UploadGdbForCreateWorkflow([FromRoute] int projectID, IFormFile file)
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

        // Clear old staging rows for this project/user and save the GeoJSON for each feature class
        var existingStaging = await DbContext.ProjectLocationStagings
            .Where(s => s.ProjectID == projectID && s.PersonID == CallingUser.PersonID)
            .ToListAsync();
        DbContext.ProjectLocationStagings.RemoveRange(existingStaging);

        foreach (var fc in featureClasses)
        {
            var geoJson = await gdalApiService.Ogr2OgrGdbLayerToGeoJson(file, fc.FeatureClassName);
            DbContext.ProjectLocationStagings.Add(new ProjectLocationStaging
            {
                ProjectID = projectID,
                PersonID = CallingUser.PersonID,
                FeatureClassName = fc.FeatureClassName,
                GeoJson = geoJson,
                ShouldImport = false
            });
        }

        await DbContext.SaveChangesAsync();
        return Ok(featureClasses);
    }

    [HttpPost("{projectID}/create-workflow/steps/location-detailed/approve-gdb")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStep>> ApproveGdbForCreateWorkflow([FromRoute] int projectID, [FromBody] GdbApproveRequest request)
    {
        var dto = await ProjectWorkflowSteps.ApproveGdbImportAsync(DbContext, projectID, CallingUser.PersonID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Create Workflow Steps - Geographic Assignments

    [HttpGet("{projectID}/create-workflow/steps/priority-landscapes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> GetCreatePriorityLandscapesStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetPriorityLandscapesStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/priority-landscapes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> SaveCreatePriorityLandscapesStep([FromRoute] int projectID, [FromBody] GeographicOverrideRequest request)
    {
        var dto = await ProjectWorkflowSteps.SavePriorityLandscapesStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpGet("{projectID}/create-workflow/steps/dnr-upland-regions")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> GetCreateDnrUplandRegionsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetDnrUplandRegionsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/dnr-upland-regions")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> SaveCreateDnrUplandRegionsStep([FromRoute] int projectID, [FromBody] GeographicOverrideRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveDnrUplandRegionsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpGet("{projectID}/create-workflow/steps/counties")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> GetCreateCountiesStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetCountiesStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/counties")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> SaveCreateCountiesStep([FromRoute] int projectID, [FromBody] GeographicOverrideRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveCountiesStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Create Workflow Steps - Organizations

    [HttpGet("{projectID}/create-workflow/steps/organizations")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectOrganizationsStep>> GetCreateOrganizationsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetOrganizationsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/organizations")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectOrganizationsStep>> SaveCreateOrganizationsStep([FromRoute] int projectID, [FromBody] ProjectOrganizationsStepRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveOrganizationsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Create Workflow Steps - Contacts

    [HttpGet("{projectID}/create-workflow/steps/contacts")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectContactsStep>> GetCreateContactsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetContactsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/contacts")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectContactsStep>> SaveCreateContactsStep([FromRoute] int projectID, [FromBody] ProjectContactsStepRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveContactsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Create Workflow Steps - Expected Funding

    [HttpGet("{projectID}/create-workflow/steps/expected-funding")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ExpectedFundingStep>> GetCreateExpectedFundingStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetExpectedFundingStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/expected-funding")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ExpectedFundingStep>> SaveCreateExpectedFundingStep([FromRoute] int projectID, [FromBody] ExpectedFundingStepRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveExpectedFundingStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Create Workflow Steps - Classifications

    [HttpGet("{projectID}/create-workflow/steps/classifications")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectClassificationsStep>> GetCreateClassificationsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetClassificationsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/create-workflow/steps/classifications")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectClassificationsStep>> SaveCreateClassificationsStep([FromRoute] int projectID, [FromBody] ProjectClassificationsStepRequest request)
    {
        var dto = await ProjectWorkflowSteps.SaveClassificationsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Create Workflow State Transitions

    [HttpPost("{projectID}/create-workflow/submit")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponse>> SubmitCreateForApproval([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequest? request)
    {
        var response = await ProjectWorkflowSteps.SubmitForApprovalAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        try
        {
            await notificationService.SendCreateSubmittedNotificationAsync(projectID, CallingUser.PersonID);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send create-submission notification for project {ProjectID}", projectID);
        }

        return Ok(response);
    }

    [HttpPost("{projectID}/create-workflow/approve")]
    [ProjectApproveFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponse>> ApproveCreate([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequest? request)
    {
        var response = await ProjectWorkflowSteps.ApproveAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        try
        {
            await notificationService.SendCreateApprovedNotificationAsync(projectID, CallingUser.PersonID);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send create-approval notification for project {ProjectID}", projectID);
        }

        return Ok(response);
    }

    [HttpPost("{projectID}/create-workflow/return")]
    [ProjectApproveFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponse>> ReturnCreate([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequest? request)
    {
        var response = await ProjectWorkflowSteps.ReturnAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        try
        {
            await notificationService.SendCreateReturnedNotificationAsync(projectID, CallingUser.PersonID, request?.Comment);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send create-return notification for project {ProjectID}", projectID);
        }

        return Ok(response);
    }

    [HttpPost("{projectID}/create-workflow/reject")]
    [ProjectApproveFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponse>> RejectCreate([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequest? request)
    {
        var response = await ProjectWorkflowSteps.RejectAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        try
        {
            await notificationService.SendCreateRejectedNotificationAsync(projectID, CallingUser.PersonID, request?.Comment);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send create-rejection notification for project {ProjectID}", projectID);
        }

        return Ok(response);
    }

    [HttpPost("{projectID}/create-workflow/withdraw")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponse>> WithdrawCreate([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequest? request)
    {
        var response = await ProjectWorkflowSteps.WithdrawAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    #endregion

    #region Update Workflow - Batch Management

    [HttpPost("{projectID}/update-workflow/start")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateBatchResponse>> StartUpdateBatch([FromRoute] int projectID)
    {
        try
        {
            var batch = await ProjectUpdateWorkflowSteps.StartBatchAsync(DbContext, projectID, CallingUser.PersonID);
            if (batch == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetCurrentUpdateBatch), new { projectID }, batch);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpGet("{projectID}/update-workflow/current")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateBatchResponse>> GetCurrentUpdateBatch([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }
        return Ok(batch);
    }

    [HttpDelete("{projectID}/update-workflow/current")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> DeleteUpdateBatch([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            await ProjectUpdateWorkflowSteps.DeleteBatchAsync(DbContext, batch.ProjectUpdateBatchID, CallingUser.PersonID);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow - Progress

    [HttpGet("{projectID}/update-workflow/progress")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<UpdateWorkflowProgressResponse>> GetUpdateWorkflowProgress([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var progress = await ProjectUpdateWorkflowProgress.GetProgressForUserAsync(DbContext, batch.ProjectUpdateBatchID, CallingUser);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    #endregion

    #region Update Workflow Steps - Basics

    [HttpGet("{projectID}/update-workflow/steps/basics")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateBasicsStep>> GetUpdateBasicsStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetBasicsStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/basics")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateBasicsStep>> SaveUpdateBasicsStep([FromRoute] int projectID, [FromBody] ProjectUpdateBasicsStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveBasicsStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Location Simple

    [HttpGet("{projectID}/update-workflow/steps/location-simple")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateLocationSimpleStep>> GetUpdateLocationSimpleStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetLocationSimpleStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/location-simple")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateLocationSimpleStep>> SaveUpdateLocationSimpleStep([FromRoute] int projectID, [FromBody] ProjectUpdateLocationSimpleStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveLocationSimpleStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Location Detailed

    [HttpGet("{projectID}/update-workflow/steps/location-detailed")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateLocationDetailedStep>> GetUpdateLocationDetailedStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetLocationDetailedStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/location-detailed")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateLocationDetailedStep>> SaveUpdateLocationDetailedStep([FromRoute] int projectID, [FromBody] ProjectUpdateLocationDetailedStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveLocationDetailedStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPost("{projectID}/update-workflow/steps/location-detailed/upload-gdb")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    [RequestSizeLimit(500_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
    public async Task<ActionResult<List<GdbFeatureClassPreview>>> UploadGdbForUpdateWorkflow([FromRoute] int projectID, IFormFile file)
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

        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var featureClasses = await gdalApiService.OgrInfoGdbToFeatureClassInfo(file);

        // Clear old staging rows for this batch/user and save the GeoJSON for each feature class
        var existingStaging = await DbContext.ProjectLocationStagingUpdates
            .Where(s => s.ProjectUpdateBatchID == batch.ProjectUpdateBatchID && s.PersonID == CallingUser.PersonID)
            .ToListAsync();
        DbContext.ProjectLocationStagingUpdates.RemoveRange(existingStaging);

        foreach (var fc in featureClasses)
        {
            var geoJson = await gdalApiService.Ogr2OgrGdbLayerToGeoJson(file, fc.FeatureClassName);
            DbContext.ProjectLocationStagingUpdates.Add(new ProjectLocationStagingUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                PersonID = CallingUser.PersonID,
                FeatureClassName = fc.FeatureClassName,
                GeoJson = geoJson,
                ShouldImport = false
            });
        }

        await DbContext.SaveChangesAsync();
        return Ok(featureClasses);
    }

    [HttpPost("{projectID}/update-workflow/steps/location-detailed/approve-gdb")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateLocationDetailedStep>> ApproveGdbForUpdateWorkflow([FromRoute] int projectID, [FromBody] GdbApproveRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.ApproveGdbImportAsync(DbContext, batch.ProjectUpdateBatchID, CallingUser.PersonID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Update Workflow Steps - Priority Landscapes

    [HttpGet("{projectID}/update-workflow/steps/priority-landscapes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateGeographicStep>> GetUpdatePriorityLandscapesStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetPriorityLandscapesStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/priority-landscapes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateGeographicStep>> SaveUpdatePriorityLandscapesStep([FromRoute] int projectID, [FromBody] ProjectUpdateGeographicStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SavePriorityLandscapesStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - DNR Upland Regions

    [HttpGet("{projectID}/update-workflow/steps/dnr-upland-regions")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateGeographicStep>> GetUpdateDnrUplandRegionsStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetDnrUplandRegionsStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/dnr-upland-regions")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateGeographicStep>> SaveUpdateDnrUplandRegionsStep([FromRoute] int projectID, [FromBody] ProjectUpdateGeographicStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveDnrUplandRegionsStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Counties

    [HttpGet("{projectID}/update-workflow/steps/counties")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateGeographicStep>> GetUpdateCountiesStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetCountiesStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/counties")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateGeographicStep>> SaveUpdateCountiesStep([FromRoute] int projectID, [FromBody] ProjectUpdateGeographicStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveCountiesStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Treatments

    [HttpGet("{projectID}/update-workflow/steps/treatments")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateTreatmentsStep>> GetUpdateTreatmentsStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetTreatmentsStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/treatments")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateTreatmentsStep>> SaveUpdateTreatmentsStep([FromRoute] int projectID, [FromBody] ProjectUpdateTreatmentsStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveTreatmentsStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpGet("{projectID}/update-workflow/treatment-areas")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<List<TreatmentAreaUpdateLookupItem>>> ListUpdateTreatmentAreas([FromRoute] int projectID)
    {
        var result = await ProjectUpdateWorkflowSteps.ListTreatmentAreasForUpdateBatchAsync(DbContext, projectID);
        return Ok(result);
    }

    [HttpGet("{projectID}/update-workflow/treatments/{treatmentUpdateID}")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<TreatmentUpdateDetail>> GetTreatmentUpdate([FromRoute] int projectID, [FromRoute] int treatmentUpdateID)
    {
        var dto = await ProjectUpdateWorkflowSteps.GetTreatmentUpdateByIDAsync(DbContext, treatmentUpdateID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPost("{projectID}/update-workflow/treatments")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<TreatmentUpdateDetail>> CreateTreatmentUpdate([FromRoute] int projectID, [FromBody] TreatmentUpdateUpsertRequest request)
    {
        try
        {
            var dto = await ProjectUpdateWorkflowSteps.CreateTreatmentUpdateAsync(DbContext, projectID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPut("{projectID}/update-workflow/treatments/{treatmentUpdateID}")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<TreatmentUpdateDetail>> UpdateTreatmentUpdate([FromRoute] int projectID, [FromRoute] int treatmentUpdateID, [FromBody] TreatmentUpdateUpsertRequest request)
    {
        try
        {
            var dto = await ProjectUpdateWorkflowSteps.UpdateTreatmentUpdateAsync(DbContext, treatmentUpdateID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Contacts

    [HttpGet("{projectID}/update-workflow/steps/contacts")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateContactsStep>> GetUpdateContactsStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetContactsStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/contacts")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateContactsStep>> SaveUpdateContactsStep([FromRoute] int projectID, [FromBody] ProjectUpdateContactsStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveContactsStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Organizations

    [HttpGet("{projectID}/update-workflow/steps/organizations")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateOrganizationsStep>> GetUpdateOrganizationsStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetOrganizationsStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/organizations")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateOrganizationsStep>> SaveUpdateOrganizationsStep([FromRoute] int projectID, [FromBody] ProjectUpdateOrganizationsStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveOrganizationsStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Expected Funding

    [HttpGet("{projectID}/update-workflow/steps/expected-funding")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateExpectedFundingStep>> GetUpdateExpectedFundingStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetExpectedFundingStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/expected-funding")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateExpectedFundingStep>> SaveUpdateExpectedFundingStep([FromRoute] int projectID, [FromBody] ProjectUpdateExpectedFundingStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveExpectedFundingStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Photos

    [HttpGet("{projectID}/update-workflow/steps/photos")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdatePhotosStep>> GetUpdatePhotosStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetPhotosStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/photos")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdatePhotosStep>> SaveUpdatePhotosStep([FromRoute] int projectID, [FromBody] ProjectUpdatePhotosStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SavePhotosStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - External Links

    [HttpGet("{projectID}/update-workflow/steps/external-links")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateExternalLinksStep>> GetUpdateExternalLinksStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetExternalLinksStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/external-links")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateExternalLinksStep>> SaveUpdateExternalLinksStep([FromRoute] int projectID, [FromBody] ProjectUpdateExternalLinksStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveExternalLinksStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Steps - Documents & Notes

    [HttpGet("{projectID}/update-workflow/steps/documents-notes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateDocumentsNotesStep>> GetUpdateDocumentsNotesStep([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var dto = await ProjectUpdateWorkflowSteps.GetDocumentsNotesStepAsync(DbContext, batch.ProjectUpdateBatchID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/update-workflow/steps/documents-notes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateDocumentsNotesStep>> SaveUpdateDocumentsNotesStep([FromRoute] int projectID, [FromBody] ProjectUpdateDocumentsNotesStepRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var dto = await ProjectUpdateWorkflowSteps.SaveDocumentsNotesStepAsync(DbContext, batch.ProjectUpdateBatchID, request, CallingUser.PersonID);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow Per-Step Diff and Revert

    [HttpGet("{projectID}/update-workflow/steps/{stepKey}/diff")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WADNR.Models.DataTransferObjects.StepDiffResponse>> GetUpdateStepDiff([FromRoute] int projectID, [FromRoute] string stepKey)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var response = await ProjectUpdateDiffs.GetStepDiffAsync(DbContext, batch.ProjectUpdateBatchID, stepKey);
        return Ok(new WADNR.Models.DataTransferObjects.StepDiffResponse
        {
            HasChanges = response.HasChanges,
            DiffHtml = response.DiffHtml
        });
    }

    [HttpPost("{projectID}/update-workflow/steps/{stepKey}/revert")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult> RevertUpdateStep([FromRoute] int projectID, [FromRoute] string stepKey)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var success = await ProjectUpdateWorkflowSteps.RevertStepAsync(DbContext, batch.ProjectUpdateBatchID, stepKey, CallingUser.PersonID);
            if (!success)
            {
                return BadRequest(new { ErrorMessage = "Failed to revert step." });
            }
            return Ok(new { Success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    #endregion

    #region Update Workflow State Transitions

    [HttpPost("{projectID}/update-workflow/submit")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponse>> SubmitUpdateForApproval([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var response = await ProjectUpdateWorkflowSteps.SubmitAsync(DbContext, batch.ProjectUpdateBatchID, CallingUser.PersonID);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        // Send notification to approvers
        try
        {
            await notificationService.SendUpdateSubmittedNotificationAsync(batch.ProjectUpdateBatchID, CallingUser.PersonID);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send submission notification for project {ProjectID}", projectID);
            // Don't fail the operation if notification fails
        }

        return Ok(response);
    }

    [HttpPost("{projectID}/update-workflow/approve")]
    [ProjectApproveFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponse>> ApproveUpdate([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var response = await ProjectUpdateWorkflowSteps.ApproveAsync(DbContext, batch.ProjectUpdateBatchID, CallingUser.PersonID);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        // Send notification to submitter and primary contact
        try
        {
            await notificationService.SendUpdateApprovedNotificationAsync(batch.ProjectUpdateBatchID, CallingUser.PersonID);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send approval notification for project {ProjectID}", projectID);
            // Don't fail the operation if notification fails
        }

        return Ok(response);
    }

    [HttpPost("{projectID}/update-workflow/return")]
    [ProjectApproveFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponse>> ReturnUpdate([FromRoute] int projectID, [FromBody] ProjectUpdateReturnRequest? request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var response = await ProjectUpdateWorkflowSteps.ReturnAsync(DbContext, batch.ProjectUpdateBatchID, CallingUser.PersonID, request);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        // Send notification to submitter and primary contact — concatenate non-null comments for the notification body
        var commentParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(request?.BasicsComment)) commentParts.Add($"Basics: {request.BasicsComment}");
        if (!string.IsNullOrWhiteSpace(request?.LocationSimpleComment)) commentParts.Add($"Location (Simple) & Geographic Areas: {request.LocationSimpleComment}");
        if (!string.IsNullOrWhiteSpace(request?.LocationDetailedComment)) commentParts.Add($"Location (Detailed): {request.LocationDetailedComment}");
        if (!string.IsNullOrWhiteSpace(request?.ExpectedFundingComment)) commentParts.Add($"Expected Funding: {request.ExpectedFundingComment}");
        if (!string.IsNullOrWhiteSpace(request?.ContactsComment)) commentParts.Add($"Contacts: {request.ContactsComment}");
        if (!string.IsNullOrWhiteSpace(request?.OrganizationsComment)) commentParts.Add($"Organizations: {request.OrganizationsComment}");
        var notificationComment = commentParts.Count > 0 ? string.Join("\n", commentParts) : null;

        try
        {
            await notificationService.SendUpdateReturnedNotificationAsync(batch.ProjectUpdateBatchID, CallingUser.PersonID, notificationComment);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send return notification for project {ProjectID}", projectID);
            // Don't fail the operation if notification fails
        }

        return Ok(response);
    }

    #endregion

    #region Update Workflow Diff

    [HttpGet("{projectID}/update-workflow/diff")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WADNR.Models.DataTransferObjects.ProjectUpdate.ProjectUpdateDiffSummary>> GetUpdateDiff([FromRoute] int projectID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var diffSummary = await ProjectUpdateDiffs.GetDiffSummaryAsync(DbContext, batch.ProjectUpdateBatchID);

        return Ok(new WADNR.Models.DataTransferObjects.ProjectUpdate.ProjectUpdateDiffSummary
        {
            BasicsDiffHtml = diffSummary.BasicsDiffHtml,
            OrganizationsDiffHtml = diffSummary.OrganizationsDiffHtml,
            ExternalLinksDiffHtml = diffSummary.ExternalLinksDiffHtml,
            NotesDiffHtml = diffSummary.NotesDiffHtml,
            ExpectedFundingDiffHtml = diffSummary.ExpectedFundingDiffHtml,
            HasBasicsChanges = diffSummary.HasBasicsChanges,
            HasOrganizationsChanges = diffSummary.HasOrganizationsChanges,
            HasExternalLinksChanges = diffSummary.HasExternalLinksChanges,
            HasNotesChanges = diffSummary.HasNotesChanges,
            HasExpectedFundingChanges = diffSummary.HasExpectedFundingChanges
        });
    }

    #endregion

    #region Block List

    [HttpPost("{projectID}/block-list")]
    [AdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult> AddToBlockList([FromRoute] int projectID, [FromBody] AddToBlockListRequest request)
    {
        var project = await DbContext.Projects
            .Include(p => p.ProjectPrograms)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null)
        {
            return NotFound();
        }

        if (!project.ProjectPrograms.Any())
        {
            return BadRequest(new { ErrorMessage = "Project must belong to at least one program." });
        }

        foreach (var pp in project.ProjectPrograms)
        {
            var entry = new ProjectImportBlockList
            {
                ProgramID = pp.ProgramID,
                ProjectGisIdentifier = request.ProjectGisIdentifier,
                ProjectName = request.ProjectName,
                ProjectID = projectID,
                Notes = request.Notes
            };
            DbContext.ProjectImportBlockLists.Add(entry);
        }

        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{projectID}/block-list")]
    [AdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult> RemoveFromBlockList([FromRoute] int projectID)
    {
        var entries = await DbContext.ProjectImportBlockLists
            .Where(b => b.ProjectID == projectID)
            .ToListAsync();

        if (!entries.Any())
        {
            return NotFound();
        }

        DbContext.ProjectImportBlockLists.RemoveRange(entries);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    #endregion
}
