using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    IOptions<WADNRConfiguration> ltInfoConfiguration)
    : SitkaController<CustomPageController>(dbContext, logger, ltInfoConfiguration)
{
    [HttpGet("{vanityUrl}")]
    [AllowAnonymous]
    public async Task<ActionResult<CustomPageDetail>> GetByVanityUrl([FromRoute] string vanityUrl)
    {
        var customPageDto = await CustomPages.GetByVanityUrlAsDetailAsync(DbContext, vanityUrl);
        return RequireNotNullThrowNotFound(customPageDto, "CustomPage", vanityUrl);
    }

    [HttpGet("navigation-section/{customPageNavigationSectionID}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CustomPageMenuItem>>> GetByCustomPageNavigationSectionID([FromRoute] int customPageNavigationSectionID)
    {
        var items = await CustomPages.GetByNavigationSectionAsMenuItemsAsync(DbContext, customPageNavigationSectionID);
        return Ok(items);
    }

    [HttpGet("menu-item")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CustomPageMenuItem>>> ListAsMenuItem()
    {
        var items = await CustomPages.ListAsMenuItemsAsync(DbContext);
        return Ok(items);
    }
}
