using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("agreements")]
public class AgreementController(
    WADNRDbContext dbContext,
    ILogger<AgreementController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<AgreementController>(dbContext, logger, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AgreementGridRow>>> List()
    {
        var agreements = await Agreements.ListAsGridRowAsync(DbContext);
        return Ok(agreements);
    }

    [HttpGet("{agreementID}")]
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
    //[AdminFeature]
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
    //[AdminFeature]
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
    //[AdminFeature]
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
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<IEnumerable<FundSourceAllocationLookupItem>>> ListFundSourceAllocations([FromRoute] int agreementID)
    {
        var items = await Agreements.ListFundSourceAllocationsAsLookupItemByAgreementIDAsync(DbContext, agreementID);
        return Ok(items);
    }

    [HttpGet("{agreementID}/projects")]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<IEnumerable<ProjectLookupItem>>> ListProjects([FromRoute] int agreementID)
    {
        var items = await Agreements.ListProjectsAsLookupItemByAgreementIDAsync(DbContext, agreementID);
        return Ok(items);
    }

    [HttpGet("{agreementID}/contacts")]
    [EntityNotFound(typeof(Agreement), "agreementID")]
    public async Task<ActionResult<IEnumerable<AgreementContactGridRow>>> ListContacts([FromRoute] int agreementID)
    {
        var items = await Agreements.ListContactsAsGridRowByAgreementIDAsync(DbContext, agreementID);
        return Ok(items);
    }
}
