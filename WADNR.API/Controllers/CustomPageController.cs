using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
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
    [HttpGet]
    [PageContentManageFeature]
    public async Task<ActionResult<List<CustomPageGridRow>>> List()
    {
        var items = await CustomPages.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{customPageID:int}")]
    [PageContentManageFeature]
    public async Task<ActionResult<CustomPageGridRow>> GetByID([FromRoute] int customPageID)
    {
        var item = await CustomPages.GetByIDAsGridRowAsync(DbContext, customPageID);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

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

    [HttpPost]
    [PageContentManageFeature]
    public async Task<ActionResult<CustomPageGridRow>> Create([FromBody] CustomPageUpsertRequest request)
    {
        var validationError = await ValidateUpsertRequest(request, null);
        if (validationError != null)
        {
            return BadRequest(validationError);
        }

        var created = await CustomPages.CreateAsync(DbContext, request);
        return Ok(created);
    }

    [HttpPut("{customPageID:int}")]
    [PageContentManageFeature]
    public async Task<ActionResult<CustomPageGridRow>> Update([FromRoute] int customPageID, [FromBody] CustomPageUpsertRequest request)
    {
        var entity = await CustomPages.GetByIDAsync(DbContext, customPageID);
        if (entity == null)
        {
            return NotFound();
        }

        var validationError = await ValidateUpsertRequest(request, customPageID);
        if (validationError != null)
        {
            return BadRequest(validationError);
        }

        var updated = await CustomPages.UpdateAsync(DbContext, entity, request);
        return Ok(updated);
    }

    [HttpPut("{customPageID:int}/content")]
    [PageContentManageFeature]
    public async Task<IActionResult> UpdateContent([FromRoute] int customPageID, [FromBody] CustomPageContentUpsertRequest request)
    {
        var entity = await CustomPages.GetByIDAsync(DbContext, customPageID);
        if (entity == null)
        {
            return NotFound();
        }

        await CustomPages.UpdateContentAsync(DbContext, entity, request);
        return Ok();
    }

    [HttpDelete("{customPageID:int}")]
    [PageContentManageFeature]
    public async Task<IActionResult> Delete([FromRoute] int customPageID)
    {
        var entity = await CustomPages.GetByIDAsync(DbContext, customPageID);
        if (entity == null)
        {
            return NotFound();
        }

        await CustomPages.DeleteAsync(DbContext, entity);
        return NoContent();
    }

    private async Task<string?> ValidateUpsertRequest(CustomPageUpsertRequest request, int? excludeID)
    {
        var duplicateName = await DbContext.CustomPages
            .AnyAsync(x => x.CustomPageDisplayName == request.CustomPageDisplayName && (excludeID == null || x.CustomPageID != excludeID));
        if (duplicateName)
        {
            return "A custom page with this display name already exists.";
        }

        var duplicateUrl = await DbContext.CustomPages
            .AnyAsync(x => x.CustomPageVanityUrl == request.CustomPageVanityUrl && (excludeID == null || x.CustomPageID != excludeID));
        if (duplicateUrl)
        {
            return "A custom page with this vanity URL already exists.";
        }

        return null;
    }
}
