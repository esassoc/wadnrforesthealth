using Microsoft.AspNetCore.Authorization;
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

namespace WADNR.API.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController(
    WADNRDbContext dbContext,
    ILogger<ProjectController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> List()
    {
        var projects = await Projects.ListAsGridRowAsync(DbContext);
        return Ok(projects);
    }

    [HttpGet("{projectID}")]
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectSimpleTree>>> ListProjectsWithNoSimpleLocation()
    {
        var projects = await Projects.ListWithNoSimpleLocationAsProjectSimpleTree(DbContext);
        return Ok(projects);
    }

    [HttpGet("{projectID}/images")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectImageGridRow>>> ListImages([FromRoute] int projectID)
    {
        var entities = await ProjectImages.ListAsGridRowAsync(DbContext, projectID);
        return Ok(entities);
    }

    [HttpGet("{projectID}/classifications")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ClassificationLookupItem>>> ListClassifications([FromRoute] int projectID)
    {
        var classifications = await Projects.ListClassificationsAsLookupItemByProjectIDAsync(DbContext, projectID);
        return Ok(classifications);
    }

    [HttpGet("{projectID}/treatments")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<TreatmentGridRow>>> ListTreatments([FromRoute] int projectID)
    {
        var treatments = await Treatments.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(treatments);
    }

    [HttpGet("{projectID}/treatment-areas")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<TreatmentAreaLookupItem>>> ListTreatmentAreas([FromRoute] int projectID)
    {
        var treatmentAreas = await Treatments.ListTreatmentAreasForProjectAsync(DbContext, projectID);
        return Ok(treatmentAreas);
    }

    [HttpGet("{projectID}/interaction-events")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<InteractionEventGridRow>>> ListInteractionEvents([FromRoute] int projectID)
    {
        var events = await InteractionEvents.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(events);
    }

    [HttpGet("{projectID}/locations/generic-layers")]
    [AllowAnonymous]
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
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectDocumentGridRow>>> ListDocuments([FromRoute] int projectID)
    {
        var documents = await ProjectDocuments.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(documents);
    }

    [HttpGet("{projectID}/notes")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectNoteGridRow>>> ListNotes([FromRoute] int projectID)
    {
        var notes = await ProjectNotes.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(notes);
    }

    [HttpGet("{projectID}/external-links")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectExternalLinkGridRow>>> ListExternalLinks([FromRoute] int projectID)
    {
        var links = await ProjectExternalLinks.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(links);
    }

    [HttpGet("{projectID}/update-history")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectUpdateHistoryGridRow>>> ListUpdateHistory([FromRoute] int projectID)
    {
        var updates = await ProjectUpdateBatches.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(updates);
    }

    [HttpGet("{projectID}/notifications")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectNotificationGridRow>>> ListNotifications([FromRoute] int projectID)
    {
        var notifications = await Notifications.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(notifications);
    }

    [HttpGet("{projectID}/audit-logs")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<IEnumerable<ProjectAuditLogGridRow>>> ListAuditLogs([FromRoute] int projectID)
    {
        var logs = await AuditLogs.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(logs);
    }

    #region Workflow Progress

    [HttpGet("{projectID}/workflow/progress")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectCreateWorkflowProgressDto>> GetWorkflowProgress([FromRoute] int projectID)
    {
        var progress = await ProjectCreateWorkflowProgress.GetProgressAsync(DbContext, projectID);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    #endregion

    #region Workflow Steps - Basics

    [HttpGet("{projectID}/workflow/steps/basics")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectBasicsStepDto>> GetBasicsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetBasicsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/basics")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectBasicsStepDto>> SaveBasicsStep([FromRoute] int projectID, [FromBody] ProjectBasicsStepRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveBasicsStepAsync(DbContext, projectID, request, CallingUser.PersonID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPost("workflow/steps/basics")]
    [ProjectEditFeature]
    public async Task<ActionResult<ProjectBasicsStepDto>> CreateProjectFromBasicsStep([FromBody] ProjectBasicsStepRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.CreateProjectFromBasicsStepAsync(DbContext, request, CallingUser.PersonID);
        return CreatedAtAction(nameof(GetBasicsStep), new { projectID = dto.ProjectID }, dto);
    }

    #endregion

    #region Workflow Steps - Location Simple

    [HttpGet("{projectID}/workflow/steps/location-simple")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationSimpleStepDto>> GetLocationSimpleStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetLocationSimpleStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/location-simple")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationSimpleStepDto>> SaveLocationSimpleStep([FromRoute] int projectID, [FromBody] LocationSimpleStepRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveLocationSimpleStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Workflow Steps - Location Detailed

    [HttpGet("{projectID}/workflow/steps/location-detailed")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStepDto>> GetLocationDetailedStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetLocationDetailedStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/location-detailed")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStepDto>> SaveLocationDetailedStep([FromRoute] int projectID, [FromBody] LocationDetailedStepRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveLocationDetailedStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Workflow Steps - Geographic Assignments

    [HttpGet("{projectID}/workflow/steps/priority-landscapes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStepDto>> GetPriorityLandscapesStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetPriorityLandscapesStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/priority-landscapes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStepDto>> SavePriorityLandscapesStep([FromRoute] int projectID, [FromBody] GeographicOverrideRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SavePriorityLandscapesStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpGet("{projectID}/workflow/steps/dnr-upland-regions")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStepDto>> GetDnrUplandRegionsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetDnrUplandRegionsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/dnr-upland-regions")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStepDto>> SaveDnrUplandRegionsStep([FromRoute] int projectID, [FromBody] GeographicOverrideRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveDnrUplandRegionsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpGet("{projectID}/workflow/steps/counties")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStepDto>> GetCountiesStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetCountiesStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/counties")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStepDto>> SaveCountiesStep([FromRoute] int projectID, [FromBody] GeographicOverrideRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveCountiesStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Workflow Steps - Organizations

    [HttpGet("{projectID}/workflow/steps/organizations")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectOrganizationsStepDto>> GetOrganizationsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetOrganizationsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/organizations")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectOrganizationsStepDto>> SaveOrganizationsStep([FromRoute] int projectID, [FromBody] ProjectOrganizationsStepRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveOrganizationsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Workflow Steps - Contacts

    [HttpGet("{projectID}/workflow/steps/contacts")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectContactsStepDto>> GetContactsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetContactsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/contacts")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectContactsStepDto>> SaveContactsStep([FromRoute] int projectID, [FromBody] ProjectContactsStepRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveContactsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Workflow Steps - Expected Funding

    [HttpGet("{projectID}/workflow/steps/expected-funding")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ExpectedFundingStepDto>> GetExpectedFundingStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetExpectedFundingStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/expected-funding")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ExpectedFundingStepDto>> SaveExpectedFundingStep([FromRoute] int projectID, [FromBody] ExpectedFundingStepRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveExpectedFundingStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Workflow Steps - Classifications

    [HttpGet("{projectID}/workflow/steps/classifications")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectClassificationsStepDto>> GetClassificationsStep([FromRoute] int projectID)
    {
        var dto = await ProjectWorkflowSteps.GetClassificationsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/workflow/steps/classifications")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectClassificationsStepDto>> SaveClassificationsStep([FromRoute] int projectID, [FromBody] ProjectClassificationsStepRequestDto request)
    {
        var dto = await ProjectWorkflowSteps.SaveClassificationsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Workflow State Transitions

    [HttpPost("{projectID}/workflow/submit")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponseDto>> SubmitForApproval([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequestDto? request)
    {
        var response = await ProjectWorkflowSteps.SubmitForApprovalAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("{projectID}/workflow/approve")]
    [ProjectApproveFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponseDto>> Approve([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequestDto? request)
    {
        var response = await ProjectWorkflowSteps.ApproveAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("{projectID}/workflow/return")]
    [ProjectApproveFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponseDto>> Return([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequestDto? request)
    {
        var response = await ProjectWorkflowSteps.ReturnAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("{projectID}/workflow/reject")]
    [ProjectApproveFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponseDto>> Reject([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequestDto? request)
    {
        var response = await ProjectWorkflowSteps.RejectAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("{projectID}/workflow/withdraw")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WorkflowStateTransitionResponseDto>> Withdraw([FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequestDto? request)
    {
        var response = await ProjectWorkflowSteps.WithdrawAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    #endregion
}
