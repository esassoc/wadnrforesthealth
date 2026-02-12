using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.ReportTemplates;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using ReportTemplateHelpers = WADNR.EFModels.Entities.ReportTemplates;

namespace WADNR.API.Controllers;

[ApiController]
[Route("report-templates")]
public class ReportTemplateController(
    WADNRDbContext dbContext,
    ILogger<ReportTemplateController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<ReportTemplateController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AdminFeature]
    public async Task<ActionResult<List<ReportTemplateGridRow>>> List()
    {
        var rows = await ReportTemplateHelpers.ListAsGridRowsAsync(DbContext);
        return Ok(rows);
    }

    [HttpGet("{reportTemplateID}")]
    [AdminFeature]
    public async Task<ActionResult<ReportTemplateDetail>> Get([FromRoute] int reportTemplateID)
    {
        var detail = await ReportTemplateHelpers.GetByIDAsDetailAsync(DbContext, reportTemplateID);
        return RequireNotNullThrowNotFound(detail, "ReportTemplate", reportTemplateID);
    }

    [HttpPost]
    [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
    [AdminFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ReportTemplateDetail>> Create(
        [FromForm] string displayName,
        [FromForm] string? description,
        [FromForm] int reportTemplateModelID,
        IFormFile fileResource)
    {
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length > 50)
        {
            return BadRequest("Display name is required and must be 50 characters or less.");
        }

        if (description?.Length > 250)
        {
            return BadRequest("Description must be 250 characters or less.");
        }

        if (fileResource == null || fileResource.Length == 0)
        {
            return BadRequest("A .docx template file is required.");
        }

        if (DbContext.ReportTemplates.Any(x => x.DisplayName == displayName))
        {
            return BadRequest($"Report Template with Name '{displayName}' already exists.");
        }

        var newFileResource = await fileService.CreateFileResource(DbContext, fileResource, CallingUser.PersonID);

        var reportTemplate = new ReportTemplate
        {
            FileResource = newFileResource,
            FileResourceID = newFileResource.FileResourceID,
            DisplayName = displayName,
            Description = description,
            ReportTemplateModelTypeID = ReportTemplateModelType.MultipleModels.ReportTemplateModelTypeID,
            ReportTemplateModelID = reportTemplateModelID,
            IsSystemTemplate = false
        };

        var validationResult = await ReportTemplateGenerator.ValidateReportTemplateAsync(reportTemplate, DbContext, Logger, fileService);
        if (!validationResult.IsValid)
        {
            return BadRequest($"{validationResult.ErrorMessage} \n <pre style='max-height: 300px; overflow: scroll;'>{validationResult.SourceCode}</pre>");
        }

        DbContext.ReportTemplates.Add(reportTemplate);
        await DbContext.SaveChangesAsync();
        await DbContext.Entry(reportTemplate).ReloadAsync();

        var detail = await ReportTemplateHelpers.GetByIDAsDetailAsync(DbContext, reportTemplate.ReportTemplateID);
        return Ok(detail);
    }

    [HttpPut("{reportTemplateID}")]
    [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
    [AdminFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ReportTemplateDetail>> Update(
        [FromRoute] int reportTemplateID,
        [FromForm] string displayName,
        [FromForm] string? description,
        [FromForm] int reportTemplateModelID,
        IFormFile? fileResource)
    {
        var reportTemplate = await ReportTemplateHelpers.GetByIDWithTrackingAsync(DbContext, reportTemplateID);
        if (reportTemplate == null)
        {
            return NotFound($"ReportTemplate with ID {reportTemplateID} does not exist!");
        }

        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length > 50)
        {
            return BadRequest("Display name is required and must be 50 characters or less.");
        }

        if (description?.Length > 250)
        {
            return BadRequest("Description must be 250 characters or less.");
        }

        reportTemplate.DisplayName = displayName;
        reportTemplate.Description = description;
        reportTemplate.ReportTemplateModelID = reportTemplateModelID;

        if (fileResource != null)
        {
            var newFileResource = await fileService.CreateFileResource(DbContext, fileResource, CallingUser.PersonID);
            reportTemplate.FileResource = newFileResource;
            reportTemplate.FileResourceID = newFileResource.FileResourceID;
        }

        var validationResult = await ReportTemplateGenerator.ValidateReportTemplateAsync(reportTemplate, DbContext, Logger, fileService);
        if (!validationResult.IsValid)
        {
            return BadRequest($"{validationResult.ErrorMessage} \n <pre style='max-height: 300px; overflow: scroll;'>{validationResult.SourceCode}</pre>");
        }

        await DbContext.SaveChangesAsync();
        await DbContext.Entry(reportTemplate).ReloadAsync();

        var detail = await ReportTemplateHelpers.GetByIDAsDetailAsync(DbContext, reportTemplate.ReportTemplateID);
        return Ok(detail);
    }

    [HttpDelete("{reportTemplateID}")]
    [AdminFeature]
    public async Task<IActionResult> Delete([FromRoute] int reportTemplateID)
    {
        var reportTemplate = await DbContext.ReportTemplates
            .Include(x => x.FileResource)
            .SingleOrDefaultAsync(x => x.ReportTemplateID == reportTemplateID);

        if (reportTemplate == null)
        {
            return NotFound();
        }

        var fileResourceGuid = reportTemplate.FileResource.FileResourceGUID;
        DbContext.ReportTemplates.Remove(reportTemplate);
        await DbContext.SaveChangesAsync();

        await fileService.DeleteFileStreamFromBlobStorageAsync(fileResourceGuid.ToString());

        return NoContent();
    }

    [HttpPost("generate-reports")]
    [Produces(@"application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [Authorize]
    public async Task<ActionResult> GenerateReports([FromBody] GenerateReportsRequest request)
    {
        var reportTemplate = await ReportTemplateHelpers.GetByIDAsync(DbContext, request.ReportTemplateID);
        if (reportTemplate == null)
        {
            return NotFound($"ReportTemplate with ID {request.ReportTemplateID} does not exist!");
        }

        if (request.ModelIDList == null || request.ModelIDList.Count == 0)
        {
            return BadRequest("At least one model ID is required.");
        }

        var reportTemplateGenerator = new ReportTemplateGenerator(reportTemplate, request.ModelIDList);
        var ext = reportTemplate.FileResource.OriginalFileExtension;
        var downloadFileName = $"{reportTemplate.FileResource.OriginalBaseFilename}{(ext.StartsWith(".") ? "" : ".")}{ext}";
        return await GenerateAndDownload(reportTemplateGenerator, downloadFileName);
    }

    [HttpPost("system/approval-letter/{projectID}")]
    [Produces(@"application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [Authorize]
    public async Task<ActionResult> GenerateApprovalLetter([FromRoute] int projectID)
    {
        var reportTemplate = await DbContext.ReportTemplates
            .AsNoTracking()
            .Include(x => x.FileResource)
            .FirstOrDefaultAsync(x => x.IsSystemTemplate && x.ReportTemplateModelID == (int)ReportTemplateModelEnum.Project);

        if (reportTemplate == null)
        {
            return NotFound("No system approval letter template found.");
        }

        var project = await DbContext.Projects.AsNoTracking().FirstOrDefaultAsync(x => x.ProjectID == projectID);
        if (project == null)
        {
            return NotFound($"Project with ID {projectID} does not exist!");
        }

        var reportTemplateGenerator = new ReportTemplateGenerator(reportTemplate, new List<int> { projectID });
        var downloadFileName = $"{project.ProjectName} - Financial Assistance Approval Letter.docx";
        return await GenerateAndDownload(reportTemplateGenerator, downloadFileName);
    }

    [HttpPost("system/invoice-payment-request/{invoicePaymentRequestID}")]
    [Produces(@"application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [Authorize]
    public async Task<ActionResult> GenerateInvoicePaymentRequest([FromRoute] int invoicePaymentRequestID)
    {
        var reportTemplate = await DbContext.ReportTemplates
            .AsNoTracking()
            .Include(x => x.FileResource)
            .FirstOrDefaultAsync(x => x.IsSystemTemplate && x.ReportTemplateModelID == (int)ReportTemplateModelEnum.InvoicePaymentRequest);

        if (reportTemplate == null)
        {
            return NotFound("No system invoice payment request template found.");
        }

        var ipr = await DbContext.InvoicePaymentRequests.AsNoTracking()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.InvoicePaymentRequestID == invoicePaymentRequestID);
        if (ipr == null)
        {
            return NotFound($"InvoicePaymentRequest with ID {invoicePaymentRequestID} does not exist!");
        }

        var reportTemplateGenerator = new ReportTemplateGenerator(reportTemplate, new List<int> { invoicePaymentRequestID });
        var downloadFileName = $"{ipr.Project.ProjectName} - Invoice Payment Request {ipr.InvoicePaymentRequestDate:MM-dd-yyyy}.docx";
        return await GenerateAndDownload(reportTemplateGenerator, downloadFileName);
    }

    [HttpGet("models")]
    [Authorize]
    public ActionResult<List<ReportTemplateModelLookupItem>> ListModels()
    {
        var models = ReportTemplateHelpers.ListModelsAsLookupItems();
        return Ok(models);
    }

    [HttpGet("by-model/{reportTemplateModelID}")]
    [Authorize]
    public async Task<ActionResult<List<ReportTemplateLookupItem>>> ListByModel([FromRoute] int reportTemplateModelID)
    {
        var templates = await ReportTemplateHelpers.ListByModelIDAsLookupItemsAsync(DbContext, reportTemplateModelID);
        return Ok(templates);
    }

    private async Task<ActionResult> GenerateAndDownload(ReportTemplateGenerator reportTemplateGenerator, string downloadFileName)
    {
        await reportTemplateGenerator.Generate(DbContext, fileService);
        var fileData = await System.IO.File.ReadAllBytesAsync(reportTemplateGenerator.GetCompilePath());
        var stream = new MemoryStream(fileData);
        return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", downloadFileName);
    }
}
