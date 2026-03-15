using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.API.ExcelSpecs;
using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.Shared;

namespace WADNR.API.Controllers;

[ApiController]
[Route("organizations")]
public class OrganizationController(
    WADNRDbContext dbContext,
    ILogger<OrganizationController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService,
    GDALAPIService gdalApiService = null)
    : SitkaController<OrganizationController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationGridRow>>> List()
    {
        var organizations = await Organizations.ListAsGridRowAsync(DbContext);
        return Ok(organizations);
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationLookupItem>>> ListLookup()
    {
        var organizations = await Organizations.ListAsLookupItemAsync(DbContext);
        return Ok(organizations);
    }

    [HttpGet("lookup-with-short-name")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationLookupItemWithShortName>>> ListLookupWithShortName()
    {
        var organizations = await Organizations.ListAsLookupItemWithShortNameAsync(DbContext);
        return Ok(organizations);
    }

    [HttpGet("lead-implementers")]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<OrganizationLookupItem>>> ListLeadImplementers()
    {
        var organizations = await Organizations.ListLeadImplementersAsLookupItemAsync(DbContext, CallingUser);
        return Ok(organizations);
    }

    [HttpGet("{organizationID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<OrganizationDetail>> Get([FromRoute] int organizationID)
    {
        var entity = await Organizations.GetByIDAsDetailAsync(DbContext, organizationID);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost]
    [UserManageFeature]
    public async Task<ActionResult<OrganizationDetail>> Create([FromBody] OrganizationUpsertRequest dto)
    {
        var created = await Organizations.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { organizationID = created.OrganizationID }, created);
    }

    [HttpPut("{organizationID}")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<OrganizationDetail>> Update([FromRoute] int organizationID, [FromBody] OrganizationUpsertRequest dto)
    {
        var updated = await Organizations.UpdateAsync(DbContext, organizationID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpPost("{organizationID}/logo")]
    [UserManageFeature]
    [Consumes("multipart/form-data")]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<OrganizationDetail>> UploadLogo([FromRoute] int organizationID, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);
        await Organizations.SetLogoAsync(DbContext, organizationID, fileResource.FileResourceID);
        var detail = await Organizations.GetByIDAsDetailAsync(DbContext, organizationID);
        return Ok(detail);
    }

    [HttpDelete("{organizationID}/logo")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<OrganizationDetail>> DeleteLogo([FromRoute] int organizationID)
    {
        await Organizations.SetLogoAsync(DbContext, organizationID, null);
        var detail = await Organizations.GetByIDAsDetailAsync(DbContext, organizationID);
        return Ok(detail);
    }

    [HttpDelete("{organizationID}")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<IActionResult> Delete([FromRoute] int organizationID)
    {
        var deleted = await Organizations.DeleteAsync(DbContext, organizationID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{organizationID}/programs")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<IEnumerable<ProgramGridRow>>> ListProgramsForOrganization([FromRoute] int organizationID)
    {
        var programs = await Programs.ListAsGridRowByOrganizationIDAsync(DbContext, organizationID);
        return Ok(programs);
    }

    [HttpGet("{organizationID}/projects")]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<IEnumerable<ProjectOrganizationDetailGridRow>>> ListProjectsForOrganization([FromRoute] int organizationID)
    {
        var projects = await Projects.ListAsOrganizationDetailGridRowForUserAsync(DbContext, organizationID, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{organizationID}/projects/pending")]
    [ProjectPendingViewFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<IEnumerable<ProjectOrganizationDetailGridRow>>> ListPendingProjectsForOrganization([FromRoute] int organizationID)
    {
        var projects = await Projects.ListPendingAsOrganizationDetailGridRowForUserAsync(DbContext, organizationID, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{organizationID}/agreements")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<IEnumerable<AgreementGridRow>>> ListAgreementsForOrganization([FromRoute] int organizationID)
    {
        var agreements = await Agreements.ListAsGridRowByOrganizationIDAsync(DbContext, organizationID);
        return Ok(agreements);
    }

    [HttpGet("{organizationID}/agreements/excel-download")]
    [ExcelDownloadFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<IActionResult> AgreementsExcelDownload([FromRoute] int organizationID)
    {
        var agreements = await Agreements.ListAsExcelRowByOrganizationIDAsync(DbContext, organizationID);
        var spec = new AgreementExcelSpec();
        var sheet = ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Agreements", spec, agreements);
        var wbm = new ExcelWorkbookMaker(sheet);
        var workbook = wbm.ToXLWorkbook();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Agreements.xlsx");
    }

    [HttpGet("{organizationID}/boundary")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<FeatureCollection>> GetBoundary([FromRoute] int organizationID)
    {
        var features = await Organizations.GetBoundaryAsFeatureCollectionAsync(DbContext, organizationID);
        return Ok(features);
    }

    [HttpDelete("{organizationID}/boundary")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<IActionResult> DeleteBoundary([FromRoute] int organizationID)
    {
        var deleted = await Organizations.DeleteBoundaryAsync(DbContext, organizationID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{organizationID}/project-locations")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<FeatureCollection>> GetProjectLocations([FromRoute] int organizationID)
    {
        var features = await Organizations.GetProjectLocationsAsFeatureCollectionAsync(DbContext, organizationID);
        return Ok(features);
    }

    [HttpPost("{organizationID}/boundary/upload-gdb")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    [RequestSizeLimit(500_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
    public async Task<ActionResult<List<GdbFeatureClassPreview>>> UploadGdbForBoundary([FromRoute] int organizationID, IFormFile file)
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

        await Organizations.ClearAndSaveBoundaryStagingAsync(DbContext, organizationID, featureClasses,
            featureClassName => gdalApiService.Ogr2OgrGdbLayerToGeoJson(file, featureClassName));

        return Ok(featureClasses);
    }

    [HttpGet("{organizationID}/boundary/staged-features")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<List<StagedFeatureLayer>>> GetStagedBoundaryFeatures([FromRoute] int organizationID)
    {
        var features = await Organizations.GetStagedBoundaryFeaturesAsync(DbContext, organizationID);
        return Ok(features);
    }

    [HttpPost("{organizationID}/boundary/approve-gdb")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<IActionResult> ApproveGdbForBoundary([FromRoute] int organizationID, [FromBody] SinglePolygonApproveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SelectedGeometryWkt))
        {
            return BadRequest(new { ErrorMessage = "A geometry selection is required." });
        }

        var success = await Organizations.ApproveBoundaryAsync(DbContext, organizationID, request.SelectedGeometryWkt);
        if (!success)
        {
            return NotFound();
        }

        return Ok();
    }
}
