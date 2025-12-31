using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("custom-rich-texts")]
public class CustomRichTextController(
    WADNRDbContext dbContext,
    ILogger<CustomRichTextController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> ltInfoConfiguration)
    : SitkaController<CustomRichTextController>(dbContext, logger, keystoneService, ltInfoConfiguration)
{
    [HttpGet("{customRichTextTypeID}")]
    public async Task<ActionResult<FirmaPageDetail>> Get([FromRoute] int customRichTextTypeID)
    {
        var customRichTextDto = await FirmaPages.GetByFirmaPageTypeAsDetailAsync(DbContext, customRichTextTypeID);
        return RequireNotNullThrowNotFound(customRichTextDto, "CustomRichText", customRichTextTypeID);
    }
}