using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("agreements")]
public class AgreementController(
    WADNRDbContext dbContext,
    ILogger<AgreementController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<AgreementController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AgreementGridRow>>> List()
    {
        var agreements = await Agreements.ListAsGridRowAsync(DbContext);
        return Ok(agreements);
    }

    [HttpGet("{agreementID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<AgreementDetail>> Get([FromRoute] int agreementID)
    {
        var entity = await Agreements.GetByIDAsDetailAsync(DbContext, agreementID);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost]
    [AgreementManageFeature]
    public async Task<ActionResult<AgreementDetail>> Create([FromBody] AgreementUpsertRequest dto)
    {
        var created = await Agreements.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { agreementID = created.AgreementID }, created);
    }

    [HttpPut("{agreementID}")]
    [AgreementManageFeature]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<AgreementDetail>> Update([FromRoute] int agreementID, [FromBody] AgreementUpsertRequest dto)
    {
        var updated = await Agreements.UpdateAsync(DbContext, agreementID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{agreementID}")]
    [AgreementManageFeature]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<IActionResult> Delete([FromRoute] int agreementID)
    {
        var deleted = await Agreements.DeleteAsync(DbContext, agreementID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{agreementID}/fund-source-allocations")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<IEnumerable<FundSourceAllocationLookupItem>>> ListFundSourceAllocations([FromRoute] int agreementID)
    {
        var items = await Agreements.ListFundSourceAllocationsAsLookupItemByAgreementIDAsync(DbContext, agreementID);
        return Ok(items);
    }

    [HttpGet("{agreementID}/projects")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<IEnumerable<ProjectLookupItem>>> ListProjects([FromRoute] int agreementID)
    {
        var items = await Agreements.ListProjectsAsLookupItemByAgreementIDAsync(DbContext, agreementID);
        return Ok(items);
    }

    [HttpGet("{agreementID}/contacts")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<IEnumerable<AgreementContactGridRow>>> ListContacts([FromRoute] int agreementID)
    {
        var items = await Agreements.ListContactsAsGridRowByAgreementIDAsync(DbContext, agreementID);
        return Ok(items);
    }

    [HttpPut("{agreementID}/fund-source-allocations")]
    [AgreementManageFeature]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<List<FundSourceAllocationLookupItem>>> UpdateFundSourceAllocations(
        [FromRoute] int agreementID,
        [FromBody] AgreementFundSourceAllocationsUpdateRequest request)
    {
        var items = await Agreements.UpdateFundSourceAllocationsAsync(DbContext, agreementID, request);
        return Ok(items);
    }

    [HttpPut("{agreementID}/projects")]
    [AgreementManageFeature]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<List<ProjectLookupItem>>> UpdateProjects(
        [FromRoute] int agreementID,
        [FromBody] AgreementProjectsUpdateRequest request)
    {
        var items = await Agreements.UpdateProjectsAsync(DbContext, agreementID, request);
        return Ok(items);
    }

    [HttpPost("upload-file")]
    [AgreementManageFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);
        return Ok(fileResource.FileResourceID);
    }

    [HttpPost("{agreementID}/contacts")]
    [AgreementManageFeature]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<AgreementContactGridRow>> CreateContact(
        [FromRoute] int agreementID,
        [FromBody] AgreementContactUpsertRequest request)
    {
        var contact = await Agreements.CreateContactAsync(DbContext, agreementID, request);
        return Ok(contact);
    }

    [HttpPut("{agreementID}/contacts/{agreementPersonID}")]
    [AgreementManageFeature]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<AgreementContactGridRow>> UpdateContact(
        [FromRoute] int agreementID,
        [FromRoute] int agreementPersonID,
        [FromBody] AgreementContactUpsertRequest request)
    {
        var contact = await Agreements.UpdateContactAsync(DbContext, agreementPersonID, request);
        return Ok(contact);
    }

    [HttpDelete("{agreementID}/contacts/{agreementPersonID}")]
    [AgreementManageFeature]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<IActionResult> DeleteContact([FromRoute] int agreementID, [FromRoute] int agreementPersonID)
    {
        var deleted = await Agreements.DeleteContactAsync(DbContext, agreementPersonID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
