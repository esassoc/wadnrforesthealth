using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("custom-pages")]
public class CustomPageController(
    WADNRDbContext dbContext,
    ILogger<CustomPageController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> ltInfoConfiguration)
    : SitkaController<CustomPageController>(dbContext, logger, keystoneService, ltInfoConfiguration)
{
    [HttpGet("{customPageID}")]
    public async Task<ActionResult<CustomPageDetail>> Get([FromRoute] int customPageID)
    {
        var customPageDto = await CustomPages.GetByCustomPageIDAsDetailAsync(DbContext, customPageID);
        return RequireNotNullThrowNotFound(customPageDto, "CustomPage", customPageID);
    }
}
