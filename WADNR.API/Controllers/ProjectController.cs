using System;
using System.IO;
using System.Net.Mail;
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
using WADNR.Common.EMail;
using WADNR.API.ExcelSpecs;
using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.EFModels.Entities;
using WADNR.EFModels.Workflows;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.Invoice;
using WADNR.Models.DataTransferObjects.InvoicePaymentRequest;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

namespace WADNR.API.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController(
    WADNRDbContext dbContext,
    ILogger<ProjectController> logger,
    IOptions<WADNRConfiguration> configuration,
    ProjectNotificationService notificationService,
    FileService fileService,
    SitkaSmtpClientService emailService,
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

    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectFeatured>>> ListFeatured()
    {
        var projects = await Projects.ListFeaturedAsync(DbContext);
        return Ok(projects);
    }


    [HttpPut("featured")]
    [AdminFeature]
    public async Task<ActionResult> UpdateFeatured([FromBody] FeaturedProjectsUpdateRequest request)
    {
        await Projects.UpdateFeaturedAsync(DbContext, request);
        return NoContent();
    }

    [HttpGet("excel-download")]
    [ExcelDownloadFeature]
    public async Task<IActionResult> ExcelDownload()
    {
        var projects = await Projects.ListAsExcelRowForUserAsync(DbContext, CallingUser);
        var projectIDs = projects.Select(p => p.ProjectID).ToList();

        var descriptions = await Projects.ListAsDescriptionExcelRowForUserAsync(DbContext, CallingUser);
        var organizations = await ProjectOrganizations.ListAllAsExcelRowAsync(DbContext, projectIDs);
        var notes = await ProjectNotes.ListAllAsExcelRowAsync(DbContext, projectIDs);
        var classifications = await ProjectClassifications.ListAllAsExcelRowAsync(DbContext, projectIDs);

        var sheets = new List<IExcelWorkbookSheetDescriptor>
        {
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Projects", new ProjectExcelSpec(), projects),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Project Descriptions", new ProjectDescriptionExcelSpec(), descriptions),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Project Contributing Organizations", new ProjectOrganizationExcelSpec(), organizations),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Project Notes", new ProjectNoteExcelSpec(), notes),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Project Themes", new ProjectClassificationExcelSpec(), classifications),
        };
        var wbm = new ExcelWorkbookMaker(sheets);
        var workbook = wbm.ToXLWorkbook();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Projects.xlsx");
    }

    [HttpGet("pending")]
    [ProjectPendingViewFeature]
    public async Task<ActionResult<IEnumerable<PendingProjectGridRow>>> ListPending()
    {
        var projects = await Projects.ListPendingAsGridRowForUserAsync(DbContext, CallingUser);
        return Ok(projects);
    }

    [HttpGet("pending/excel-download")]
    [ExcelDownloadFeature]
    public async Task<IActionResult> PendingExcelDownload()
    {
        var projects = await Projects.ListPendingAsExcelRowForUserAsync(DbContext, CallingUser);
        var projectIDs = projects.Select(p => p.ProjectID).ToList();

        var descriptions = await Projects.ListPendingAsDescriptionExcelRowForUserAsync(DbContext, CallingUser);
        var organizations = await ProjectOrganizations.ListAllAsExcelRowAsync(DbContext, projectIDs);
        var notes = await ProjectNotes.ListAllAsExcelRowAsync(DbContext, projectIDs);
        var classifications = await ProjectClassifications.ListAllAsExcelRowAsync(DbContext, projectIDs);

        var sheets = new List<IExcelWorkbookSheetDescriptor>
        {
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Pending Projects", new ProjectExcelSpec(), projects),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Project Descriptions", new ProjectDescriptionExcelSpec(), descriptions),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Project Contributing Organizations", new ProjectOrganizationExcelSpec(), organizations),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Project Notes", new ProjectNoteExcelSpec(), notes),
            ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Project Themes", new ProjectClassificationExcelSpec(), classifications),
        };
        var wbm = new ExcelWorkbookMaker(sheets);
        var workbook = wbm.ToXLWorkbook();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PendingProjects.xlsx");
    }

    [HttpGet("update-status")]
    [NormalUserFeature]
    public async Task<ActionResult<List<ProjectUpdateStatusGridRow>>> ListUpdateStatus()
    {
        var rows = await Projects.ListUpdateStatusForUserAsync(DbContext, CallingUser);
        return Ok(rows);
    }

    [HttpGet("no-contact-count")]
    [AdminFeature]
    public async Task<ActionResult<int>> GetNoContactCount()
    {
        var count = await Projects.GetProjectsWithNoContactCountAsync(DbContext);
        return Ok(count);
    }

    [HttpGet("lookup")]
    [NormalUserFeature]
    public async Task<ActionResult<List<ProjectLookupItem>>> ListLookup()
    {
        var projects = await Projects.ListAsLookupItemAsync(DbContext);
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

    [HttpGet("{projectID}/map-popup-html")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ContentResult> GetAsMapPopupHtml([FromRoute] int projectID)
    {
        var popup = await Projects.GetByIDAsMapPopupHtmlAsync(DbContext, projectID);
        if (popup == null)
        {
            return new ContentResult { Content = "Not found", ContentType = "text/html", StatusCode = 404 };
        }

        var encode = (string s) => System.Net.WebUtility.HtmlEncode(s);

        var classificationHtml = "";
        var classificationsBySystem = popup.Classifications
            .GroupBy(c => c.ClassificationSystemName ?? "Classifications")
            .OrderBy(g => g.Key);
        foreach (var group in classificationsBySystem)
        {
            var names = string.Join(", ", group.Select(c => encode(c.DisplayName)));
            classificationHtml += $"<dt><strong>{encode(group.Key)}:</strong></dt>\n<dd>{names}</dd>\n";
        }

        var leadImplHtml = popup.LeadImplementerOrganizationID != null
            ? $@"<a target=""_blank"" href=""/organizations/{popup.LeadImplementerOrganizationID}"">{encode(popup.LeadImplementerName)}</a>"
            : "";

        var projectUrl = $"/projects/{popup.ProjectID}";
        var html = $@"<style>
    .row {{
        margin-bottom: 5px;
        padding-left: 10px;
    }}

    dl.inline {{
        margin-bottom: 0;
    }}

    dl.inline dd {{
        display: inline;
        margin: 0;
        max-width: 200px;
        margin-bottom: 8px;
    }}

    dl.inline dd:after {{
        display: block;
        content: '';
    }}

    dl.inline dt {{
        display: inline-block;
    }}

    .leaflet-popup-content {{
        width: 300px !important;
    }}
</style>
<div style=""width:300px"">
    <div style=""font-weight: bold; border-bottom: 1px solid lightgray; margin-bottom: 10px"">
        <a target=""_blank"" href=""{projectUrl}"">{encode(popup.ProjectName)}</a>
    </div>
    <div class=""row"">
        <div class=""col-xs-12"">
            <dl class=""inline"">
                <dt><strong>Duration:</strong></dt>
                <dd>{encode(popup.Duration ?? "")}</dd>
                <dt><strong>Stage:</strong></dt>
                <dd>{encode(popup.ProjectStageName)}</dd>
                <dt><strong>Project Type:</strong></dt>
                <dd>{encode(popup.ProjectTypeName)}</dd>
                <dt><strong>Lead Implementer Organization:</strong></dt>
                <dd>{leadImplHtml}</dd>
                {classificationHtml}
            </dl>
        </div>
    </div>
    <div class=""row"">
        <div class=""col-xs-12"" style=""text-align: center"">
            <span>
                For Project information &amp; results, see the
                <a target=""_blank"" href=""{projectUrl}"">Project Detail page</a>
            </span>
        </div>
    </div>
</div>";

        return Content(html, "text/html");
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
        var fileResourceGuids = await Projects.DeleteAsync(DbContext, projectID);

        foreach (var guid in fileResourceGuids)
        {
            await fileService.DeleteFileStreamFromBlobStorageAsync(guid.ToString());
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
    public async Task<ActionResult<IEnumerable<ProjectClassificationDetailItem>>> ListClassifications([FromRoute] int projectID)
    {
        var classifications = await Projects.ListClassificationsAsDetailItemByProjectIDForUserAsync(DbContext, projectID, CallingUser);
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
        if (project == null)
        {
            return NotFound();
        }
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
            return Ok(genericLayers);
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

    [HttpGet("{projectID}/invoices")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<List<InvoiceGridRow>>> ListInvoices([FromRoute] int projectID)
    {
        var invoices = await Invoices.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(invoices);
    }

    [HttpGet("{projectID}/invoice-payment-requests")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<List<InvoicePaymentRequestGridRow>>> ListInvoicePaymentRequests([FromRoute] int projectID)
    {
        var paymentRequests = await InvoicePaymentRequests.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(paymentRequests);
    }

    [HttpPut("{projectID}/external-links")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<List<ProjectExternalLinkGridRow>>> SaveAllExternalLinks(
        [FromRoute] int projectID,
        [FromBody] ProjectExternalLinkSaveRequest request)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        foreach (var link in request.ExternalLinks)
        {
            if (string.IsNullOrWhiteSpace(link.ExternalLinkLabel))
            {
                return BadRequest("Link label is required.");
            }
            if (string.IsNullOrWhiteSpace(link.ExternalLinkUrl))
            {
                return BadRequest("Link URL is required.");
            }
            if (!link.ExternalLinkUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !link.ExternalLinkUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                link.ExternalLinkUrl = "https://" + link.ExternalLinkUrl;
            }
        }

        var result = await ProjectExternalLinks.SaveAllAsync(DbContext, projectID, request);
        return Ok(result);
    }

    [HttpPut("{projectID}/organizations")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<List<ProjectOrganizationItem>>> SaveAllOrganizations(
        [FromRoute] int projectID,
        [FromBody] ProjectOrganizationSaveRequest request)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        var relationshipTypes = await DbContext.RelationshipTypes.AsNoTracking().ToListAsync();
        var rtLookup = relationshipTypes.ToDictionary(rt => rt.RelationshipTypeID);

        foreach (var org in request.Organizations)
        {
            if (!rtLookup.ContainsKey(org.RelationshipTypeID))
            {
                return BadRequest($"Relationship type with ID {org.RelationshipTypeID} not found.");
            }
        }

        var groupedByType = request.Organizations.GroupBy(o => o.RelationshipTypeID);
        foreach (var group in groupedByType)
        {
            var rt = rtLookup[group.Key];
            if ((rt.CanOnlyBeRelatedOnceToAProject || rt.IsPrimaryContact) && group.Count() > 1)
            {
                return BadRequest($"Relationship type '{rt.RelationshipTypeName}' can only have one organization associated.");
            }
        }

        var result = await ProjectOrganizations.SaveAllAsync(DbContext, projectID, request);
        return Ok(result);
    }

    [HttpPut("{projectID}/contacts")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<List<ProjectPersonItem>>> SaveAllContacts(
        [FromRoute] int projectID,
        [FromBody] ProjectContactSaveRequest request)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        var primaryContactCount = request.Contacts
            .Count(c => c.ProjectPersonRelationshipTypeID == (int)ProjectPersonRelationshipTypeEnum.PrimaryContact);
        if (primaryContactCount > 1)
        {
            return BadRequest("Only one Primary Contact is allowed per project.");
        }

        var result = await ProjectPeople.SaveAllAsync(DbContext, projectID, request);
        return Ok(result);
    }

    [HttpGet("{projectID}/basics/edit")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<ProjectBasicsEditData>> GetBasicsEditData([FromRoute] int projectID)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        var result = await Projects.GetBasicsEditDataAsync(DbContext, projectID);
        return Ok(result);
    }

    [HttpPut("{projectID}/basics")]
    [ProjectEditAsAdminFeature]
    public async Task<IActionResult> SaveBasics(
        [FromRoute] int projectID,
        [FromBody] ProjectBasicsSaveRequest request)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        await Projects.SaveBasicsAsync(DbContext, projectID, request);
        return NoContent();
    }

    [HttpPut("{projectID}/tags")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<List<TagLookupItem>>> SaveAllTags(
        [FromRoute] int projectID,
        [FromBody] ProjectTagSaveRequest request)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        var result = await ProjectTags.SaveAllAsync(DbContext, projectID, request);
        return Ok(result);
    }

    [HttpGet("{projectID}/classifications/edit")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<List<ProjectClassificationDetailItem>>> ListClassificationsForEdit([FromRoute] int projectID)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        var result = await ProjectClassifications.ListForProjectAsDetailItemAsync(DbContext, projectID);
        return Ok(result);
    }

    [HttpPut("{projectID}/classifications")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<List<ProjectClassificationDetailItem>>> SaveAllClassifications(
        [FromRoute] int projectID,
        [FromBody] ProjectClassificationSaveRequest request)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        var result = await ProjectClassifications.SaveAllAsync(DbContext, projectID, request);
        return Ok(result);
    }

    [HttpGet("{projectID}/funding")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<ProjectFundingDetail>> GetFunding([FromRoute] int projectID)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        var result = await ProjectFunding.GetForProjectAsync(DbContext, projectID);
        return Ok(result);
    }

    [HttpPut("{projectID}/funding")]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<ProjectFundingDetail>> SaveFunding(
        [FromRoute] int projectID,
        [FromBody] ProjectFundingSaveRequest request)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        var result = await ProjectFunding.SaveAllAsync(DbContext, projectID, request);
        return Ok(result);
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

    [HttpGet("{projectID}/update-history/{projectUpdateBatchID}/diff")]
    [AllowAnonymous]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<WADNR.Models.DataTransferObjects.ProjectUpdate.ProjectUpdateDiffSummary>> GetUpdateHistoryDiff(
        [FromRoute] int projectID, [FromRoute] int projectUpdateBatchID)
    {
        var diffSummary = await ProjectUpdateDiffs.GetDiffSummaryAsync(DbContext, projectUpdateBatchID);
        return Ok(diffSummary);
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
        var dto = await ProjectCreateWorkflowSteps.GetBasicsStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveBasicsStepAsync(DbContext, projectID, request, CallingUser.PersonID);
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
        var dto = await ProjectCreateWorkflowSteps.CreateProjectFromBasicsStepAsync(DbContext, request, CallingUser.PersonID);
        return CreatedAtAction(nameof(GetCreateBasicsStep), new { projectID = dto.ProjectID }, dto);
    }

    #endregion

    #region Create Workflow Steps - Location Simple

    [HttpGet("{projectID}/create-workflow/steps/location-simple")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationSimpleStep>> GetCreateLocationSimpleStep([FromRoute] int projectID)
    {
        var dto = await ProjectCreateWorkflowSteps.GetLocationSimpleStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveLocationSimpleStepAsync(DbContext, projectID, request);
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
        var dto = await ProjectCreateWorkflowSteps.GetLocationDetailedStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveLocationDetailedStepAsync(DbContext, projectID, request);
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

        List<GdbFeatureClassPreview> featureClasses;
        try
        {
            featureClasses = await gdalApiService.OgrInfoGdbToFeatureClassInfo(file);

            // Clear old staging rows for this project/user and save the GeoJSON for each feature class
            var existingStaging = await DbContext.ProjectLocationStagings
                .Where(s => s.ProjectID == projectID && s.PersonID == CallingUser.PersonID)
                .ToListAsync();
            DbContext.ProjectLocationStagings.RemoveRange(existingStaging);

            foreach (var fc in featureClasses)
            {
                var geoJson = await gdalApiService.Ogr2OgrGdbLayerToGeoJson(file, fc.FeatureClassName);
                fc.GeoJson = geoJson;
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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GDB import failed for project {ProjectID}", projectID);
            return StatusCode(500, new { ErrorMessage = "Failed to process the GDB file. The file may be corrupt or the import service may be unavailable. Please try again." });
        }

        return Ok(featureClasses);
    }

    [HttpPost("{projectID}/create-workflow/steps/location-detailed/approve-gdb")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStep>> ApproveGdbForCreateWorkflow([FromRoute] int projectID, [FromBody] GdbApproveRequest request)
    {
        var dto = await ProjectCreateWorkflowSteps.ApproveGdbImportAsync(DbContext, projectID, CallingUser.PersonID, request);
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
        var dto = await ProjectCreateWorkflowSteps.GetPriorityLandscapesStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SavePriorityLandscapesStepAsync(DbContext, projectID, request);
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
        var dto = await ProjectCreateWorkflowSteps.GetDnrUplandRegionsStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveDnrUplandRegionsStepAsync(DbContext, projectID, request);
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
        var dto = await ProjectCreateWorkflowSteps.GetCountiesStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveCountiesStepAsync(DbContext, projectID, request);
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
        var dto = await ProjectCreateWorkflowSteps.GetOrganizationsStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveOrganizationsStepAsync(DbContext, projectID, request);
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
        var dto = await ProjectCreateWorkflowSteps.GetContactsStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveContactsStepAsync(DbContext, projectID, request);
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
        var dto = await ProjectCreateWorkflowSteps.GetExpectedFundingStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveExpectedFundingStepAsync(DbContext, projectID, request);
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
        var dto = await ProjectCreateWorkflowSteps.GetClassificationsStepAsync(DbContext, projectID);
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
        var dto = await ProjectCreateWorkflowSteps.SaveClassificationsStepAsync(DbContext, projectID, request);
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
        var response = await ProjectCreateWorkflowSteps.SubmitForApprovalAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
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
        var response = await ProjectCreateWorkflowSteps.ApproveAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
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
        var response = await ProjectCreateWorkflowSteps.ReturnAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
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
        var response = await ProjectCreateWorkflowSteps.RejectAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
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
        var response = await ProjectCreateWorkflowSteps.WithdrawAsync(DbContext, projectID, CallingUser.PersonID, request?.Comment);
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
    public async Task<ActionResult<ProjectUpdateBatchDetail>> StartUpdateBatch([FromRoute] int projectID)
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
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, "Database error starting update batch for project {ProjectID}", projectID);
            return BadRequest(new { ErrorMessage = "Failed to start project update due to a data error. Please contact support if this persists." });
        }
    }

    [HttpGet("{projectID}/update-workflow/current")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<ProjectUpdateBatchDetail>> GetCurrentUpdateBatch([FromRoute] int projectID)
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

    #region Update Workflow - History

    [HttpGet("{projectID}/update-workflow/history")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<List<ProjectUpdateHistoryEntry>>> ListUpdateBatchHistory([FromRoute] int projectID)
    {
        var entries = await ProjectUpdateBatches.ListCurrentBatchHistoryAsync(DbContext, projectID);
        return Ok(entries);
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

        List<GdbFeatureClassPreview> featureClasses;
        try
        {
            featureClasses = await gdalApiService.OgrInfoGdbToFeatureClassInfo(file);

            // Clear old staging rows for this batch/user and save the GeoJSON for each feature class
            var existingStaging = await DbContext.ProjectLocationStagingUpdates
                .Where(s => s.ProjectUpdateBatchID == batch.ProjectUpdateBatchID && s.PersonID == CallingUser.PersonID)
                .ToListAsync();
            DbContext.ProjectLocationStagingUpdates.RemoveRange(existingStaging);

            foreach (var fc in featureClasses)
            {
                var geoJson = await gdalApiService.Ogr2OgrGdbLayerToGeoJson(file, fc.FeatureClassName);
                fc.GeoJson = geoJson;
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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GDB import failed for project {ProjectID}", projectID);
            return StatusCode(500, new { ErrorMessage = "Failed to process the GDB file. The file may be corrupt or the import service may be unavailable. Please try again." });
        }

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

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".gif", ".png"
    };

    [HttpPost("{projectID}/update-workflow/steps/photos/images")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateUpdatePhotoImage(
        [FromRoute] int projectID,
        [FromForm] string caption,
        [FromForm] string credit,
        [FromForm] int? projectImageTimingID,
        [FromForm] bool excludeFromFactSheet,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Image file is required.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedImageExtensions.Contains(extension))
        {
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedImageExtensions)}");
        }

        if (string.IsNullOrWhiteSpace(caption) || caption.Length > 200)
        {
            return BadRequest("Caption is required and must be 200 characters or less.");
        }

        if (string.IsNullOrWhiteSpace(credit) || credit.Length > 200)
        {
            return BadRequest("Credit is required and must be 200 characters or less.");
        }

        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);

        await ProjectUpdateWorkflowSteps.CreatePhotoUpdateAsync(
            DbContext,
            batch.ProjectUpdateBatchID,
            fileResource.FileResourceID,
            caption.Trim(),
            credit.Trim(),
            projectImageTimingID,
            excludeFromFactSheet);

        return Ok(new { Success = true });
    }

    [HttpPut("{projectID}/update-workflow/steps/photos/images/{projectImageUpdateID}")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> UpdateUpdatePhotoImage(
        [FromRoute] int projectID,
        [FromRoute] int projectImageUpdateID,
        [FromBody] ProjectImageUpsertRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Caption) || request.Caption.Length > 200)
        {
            return BadRequest("Caption is required and must be 200 characters or less.");
        }

        if (string.IsNullOrWhiteSpace(request.Credit) || request.Credit.Length > 200)
        {
            return BadRequest("Credit is required and must be 200 characters or less.");
        }

        try
        {
            await ProjectUpdateWorkflowSteps.UpdatePhotoUpdateAsync(DbContext, projectImageUpdateID, request);
            return Ok(new { Success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpDelete("{projectID}/update-workflow/steps/photos/images/{projectImageUpdateID}")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> DeleteUpdatePhotoImage(
        [FromRoute] int projectID,
        [FromRoute] int projectImageUpdateID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var fileResourceGuid = await ProjectUpdateWorkflowSteps.DeletePhotoUpdateAsync(DbContext, projectImageUpdateID);

            if (fileResourceGuid != Guid.Empty)
            {
                await fileService.DeleteFileStreamFromBlobStorageAsync(fileResourceGuid.ToString());
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPost("{projectID}/update-workflow/steps/photos/images/{projectImageUpdateID}/set-key-photo")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> SetKeyPhotoUpdatePhoto(
        [FromRoute] int projectID,
        [FromRoute] int projectImageUpdateID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            await ProjectUpdateWorkflowSteps.SetKeyPhotoUpdateAsync(DbContext, batch.ProjectUpdateBatchID, projectImageUpdateID);
            return Ok();
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

    private static readonly HashSet<string> AllowedDocumentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".zip", ".doc", ".docx", ".xls", ".xlsx"
    };

    [HttpPost("{projectID}/update-workflow/steps/documents-notes/documents")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> CreateUpdateDocument(
        [FromRoute] int projectID,
        [FromForm] string displayName,
        [FromForm] string? description,
        [FromForm] int? projectDocumentTypeID,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Document file is required.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedDocumentExtensions.Contains(extension))
        {
            return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", AllowedDocumentExtensions)}");
        }

        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);

        await ProjectUpdateWorkflowSteps.CreateDocumentUpdateAsync(
            DbContext,
            batch.ProjectUpdateBatchID,
            fileResource.FileResourceID,
            displayName.Trim(),
            description?.Trim(),
            projectDocumentTypeID);

        return Ok(new { Success = true });
    }

    [HttpPut("{projectID}/update-workflow/steps/documents-notes/documents/{projectDocumentUpdateID}")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> UpdateUpdateDocument(
        [FromRoute] int projectID,
        [FromRoute] int projectDocumentUpdateID,
        [FromBody] ProjectDocumentUpsertRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 200)
        {
            return BadRequest("Display name is required and must be 200 characters or less.");
        }

        try
        {
            await ProjectUpdateWorkflowSteps.UpdateDocumentUpdateAsync(DbContext, projectDocumentUpdateID, request);
            return Ok(new { Success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpDelete("{projectID}/update-workflow/steps/documents-notes/documents/{projectDocumentUpdateID}")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> DeleteUpdateDocument(
        [FromRoute] int projectID,
        [FromRoute] int projectDocumentUpdateID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            var fileResourceGuid = await ProjectUpdateWorkflowSteps.DeleteDocumentUpdateAsync(DbContext, projectDocumentUpdateID);

            if (fileResourceGuid != Guid.Empty)
            {
                await fileService.DeleteFileStreamFromBlobStorageAsync(fileResourceGuid.ToString());
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPost("{projectID}/update-workflow/steps/documents-notes/notes")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> CreateUpdateNote(
        [FromRoute] int projectID,
        [FromBody] ProjectNoteUpsertRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Note) || request.Note.Length > 8000)
        {
            return BadRequest("Note is required and must be 8000 characters or less.");
        }

        await ProjectUpdateWorkflowSteps.CreateNoteUpdateAsync(
            DbContext,
            batch.ProjectUpdateBatchID,
            request.Note.Trim(),
            CallingUser.PersonID);

        return Ok(new { Success = true });
    }

    [HttpPut("{projectID}/update-workflow/steps/documents-notes/notes/{projectNoteUpdateID}")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> UpdateUpdateNote(
        [FromRoute] int projectID,
        [FromRoute] int projectNoteUpdateID,
        [FromBody] ProjectNoteUpsertRequest request)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Note) || request.Note.Length > 8000)
        {
            return BadRequest("Note is required and must be 8000 characters or less.");
        }

        try
        {
            await ProjectUpdateWorkflowSteps.UpdateNoteUpdateAsync(DbContext, projectNoteUpdateID, request.Note.Trim());
            return Ok(new { Success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpDelete("{projectID}/update-workflow/steps/documents-notes/notes/{projectNoteUpdateID}")]
    [ProjectEditFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> DeleteUpdateNote(
        [FromRoute] int projectID,
        [FromRoute] int projectNoteUpdateID)
    {
        var batch = await ProjectUpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
        if (batch == null)
        {
            return NotFound();
        }

        try
        {
            await ProjectUpdateWorkflowSteps.DeleteNoteUpdateAsync(DbContext, projectNoteUpdateID);
            return NoContent();
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
        return Ok(response);
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

    #region Direct Edit - Location Simple

    [HttpGet("{projectID}/location-simple")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationSimpleStep>> GetLocationSimple([FromRoute] int projectID)
    {
        var dto = await ProjectCreateWorkflowSteps.GetLocationSimpleStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/location-simple")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationSimpleStep>> SaveLocationSimple([FromRoute] int projectID, [FromBody] LocationSimpleStepRequest request)
    {
        var dto = await Projects.SaveLocationSimpleAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Direct Edit - Location Detailed

    [HttpGet("{projectID}/location-detailed")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStep>> GetLocationDetailed([FromRoute] int projectID)
    {
        var dto = await ProjectCreateWorkflowSteps.GetLocationDetailedStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/location-detailed")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStep>> SaveLocationDetailed([FromRoute] int projectID, [FromBody] LocationDetailedStepRequest request)
    {
        try
        {
            var dto = await Projects.SaveLocationDetailedAsync(DbContext, projectID, request);
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

    [HttpPost("{projectID}/location-detailed/upload-gdb")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    [RequestSizeLimit(500_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
    public async Task<ActionResult<List<GdbFeatureClassPreview>>> UploadGdbForDirectEdit([FromRoute] int projectID, IFormFile file)
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

        List<GdbFeatureClassPreview> featureClasses;
        try
        {
            featureClasses = await gdalApiService.OgrInfoGdbToFeatureClassInfo(file);

            var existingStaging = await DbContext.ProjectLocationStagings
                .Where(s => s.ProjectID == projectID && s.PersonID == CallingUser.PersonID)
                .ToListAsync();
            DbContext.ProjectLocationStagings.RemoveRange(existingStaging);

            foreach (var fc in featureClasses)
            {
                var geoJson = await gdalApiService.Ogr2OgrGdbLayerToGeoJson(file, fc.FeatureClassName);
                fc.GeoJson = geoJson;
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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GDB import failed for project {ProjectID}", projectID);
            return StatusCode(500, new { ErrorMessage = "Failed to process the GDB file. The file may be corrupt or the import service may be unavailable. Please try again." });
        }

        return Ok(featureClasses);
    }

    [HttpPost("{projectID}/location-detailed/approve-gdb")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<LocationDetailedStep>> ApproveGdbForDirectEdit([FromRoute] int projectID, [FromBody] GdbApproveRequest request)
    {
        var dto = await Projects.ApproveGdbImportDirectEditAsync(DbContext, projectID, CallingUser.PersonID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Direct Edit - Geographic Areas

    [HttpGet("{projectID}/priority-landscapes")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> GetPriorityLandscapes([FromRoute] int projectID)
    {
        var dto = await ProjectCreateWorkflowSteps.GetPriorityLandscapesStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/priority-landscapes")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> SavePriorityLandscapes([FromRoute] int projectID, [FromBody] GeographicOverrideRequest request)
    {
        var dto = await ProjectCreateWorkflowSteps.SavePriorityLandscapesStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpGet("{projectID}/dnr-upland-regions")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> GetDnrUplandRegions([FromRoute] int projectID)
    {
        var dto = await ProjectCreateWorkflowSteps.GetDnrUplandRegionsStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/dnr-upland-regions")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> SaveDnrUplandRegions([FromRoute] int projectID, [FromBody] GeographicOverrideRequest request)
    {
        var dto = await ProjectCreateWorkflowSteps.SaveDnrUplandRegionsStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpGet("{projectID}/counties")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> GetCounties([FromRoute] int projectID)
    {
        var dto = await ProjectCreateWorkflowSteps.GetCountiesStepAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/counties")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<GeographicAssignmentStep>> SaveCounties([FromRoute] int projectID, [FromBody] GeographicOverrideRequest request)
    {
        var dto = await ProjectCreateWorkflowSteps.SaveCountiesStepAsync(DbContext, projectID, request);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    #endregion

    #region Direct Edit - Map Extent

    [HttpGet("{projectID}/map-extent")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<ActionResult<MapExtentStep>> GetMapExtent([FromRoute] int projectID)
    {
        var dto = await Projects.GetMapExtentAsync(DbContext, projectID);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPut("{projectID}/map-extent")]
    [ProjectEditAsAdminFeature]
    [EntityNotFound(typeof(Project), "projectID")]
    public async Task<IActionResult> SaveMapExtent([FromRoute] int projectID, [FromBody] MapExtentSaveRequest request)
    {
        var project = await DbContext.Projects.FindAsync(projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} not found.");
        }

        await Projects.SaveMapExtentAsync(DbContext, projectID, request);
        return NoContent();
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

    #region Project Update Notifications

    [HttpGet("people-receiving-reminders")]
    [AdminFeature]
    public async Task<ActionResult<List<PeopleReceivingReminderGridRow>>> ListPeopleReceivingReminders()
    {
        var rows = await ProjectUpdateConfigurations.ListPeopleReceivingRemindersAsync(DbContext);
        return Ok(rows);
    }

    [HttpPost("send-custom-notification")]
    [AdminFeature]
    public async Task<ActionResult> SendCustomNotification([FromBody] CustomNotificationRequest request)
    {
        if (request.PersonIDList == null || !request.PersonIDList.Any())
        {
            return BadRequest("At least one person must be selected.");
        }

        var peopleToNotify = await DbContext.People
            .Where(p => request.PersonIDList.Contains(p.PersonID) && !string.IsNullOrEmpty(p.Email))
            .ToListAsync();

        if (!peopleToNotify.Any())
        {
            return BadRequest("No valid email recipients found.");
        }

        var message = new MailMessage
        {
            Subject = request.Subject,
            Body = request.NotificationContent ?? string.Empty,
            IsBodyHtml = true
        };

        foreach (var person in peopleToNotify)
        {
            message.To.Add(new MailAddress(person.Email, $"{person.FirstName} {person.LastName}".Trim()));
        }

        await emailService.Send(message);

        var notificationDate = DateTime.Now;
        foreach (var person in peopleToNotify)
        {
            DbContext.Notifications.Add(new Notification
            {
                NotificationTypeID = (int)NotificationTypeEnum.Custom,
                PersonID = person.PersonID,
                NotificationDate = notificationDate
            });
        }
        await DbContext.SaveChangesAsync();

        return Ok(new { recipientCount = peopleToNotify.Count });
    }

    [HttpPost("send-preview-notification")]
    [AdminFeature]
    public async Task<ActionResult> SendPreviewNotification([FromBody] CustomNotificationRequest request)
    {
        var currentUser = CallingUser;
        if (string.IsNullOrEmpty(currentUser?.Email))
        {
            return BadRequest("Current user does not have an email address.");
        }

        var message = new MailMessage
        {
            Subject = request.Subject,
            Body = request.NotificationContent ?? string.Empty,
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(currentUser.Email, currentUser.FullName));

        await emailService.Send(message);

        return Ok(new { previewSentTo = currentUser.Email });
    }

    #endregion
}
