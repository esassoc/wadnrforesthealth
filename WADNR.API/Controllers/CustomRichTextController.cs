using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("custom-rich-texts")]
public class CustomRichTextController(
    WADNRDbContext dbContext,
    ILogger<CustomRichTextController> logger,
    IOptions<WADNRConfiguration> ltInfoConfiguration)
    : SitkaController<CustomRichTextController>(dbContext, logger, ltInfoConfiguration)
{
    [HttpGet]
    [PageContentManageFeature]
    public async Task<ActionResult<List<FirmaPageGridRow>>> List()
    {
        var items = await FirmaPages.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{customRichTextTypeID}")]
    [AllowAnonymous]
    public async Task<ActionResult<FirmaPageDetail>> Get([FromRoute] int customRichTextTypeID)
    {
        var customRichTextDto = await FirmaPages.GetByFirmaPageTypeAsDetailAsync(DbContext, customRichTextTypeID);
        return RequireNotNullThrowNotFound(customRichTextDto, "CustomRichText", customRichTextTypeID);
    }

    [HttpPut("{customRichTextTypeID}")]
    [PageContentManageFeature]
    public async Task<ActionResult<FirmaPageDetail>> Update(
        [FromRoute] int customRichTextTypeID,
        [FromBody] FirmaPageUpsertRequest upsertRequest)
    {
        var updated = await FirmaPages.UpdateAsync(DbContext, customRichTextTypeID, upsertRequest);
        return RequireNotNullThrowNotFound(updated, "CustomRichText", customRichTextTypeID);
    }
}