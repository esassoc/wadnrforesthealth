using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNRForestHealthTracker.API.Services;
using WADNRForestHealthTracker.EFModels.Entities;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.API.Controllers;

[ApiController]
[Route("custom-rich-texts")]
public class CustomRichTextController(
    WADNRForestHealthTrackerDbContext dbContext,
    ILogger<CustomRichTextController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRForestHealthTrackerConfiguration> ltInfoConfiguration)
    : SitkaController<CustomRichTextController>(dbContext, logger, keystoneService, ltInfoConfiguration)
{
    [HttpGet("{customRichTextTypeID}")]
    public async Task<ActionResult<FirmaPageDetail>> Get([FromRoute] int customRichTextTypeID)
    {
        var customRichTextDto = await FirmaPages.GetByFirmaPageTypeAsDetailAsync(DbContext, customRichTextTypeID);
        return RequireNotNullThrowNotFound(customRichTextDto, "CustomRichText", customRichTextTypeID);
    }
}