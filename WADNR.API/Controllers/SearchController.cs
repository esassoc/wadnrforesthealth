using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("search")]
public class SearchController(
    WADNRDbContext dbContext,
    ILogger<SearchController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<SearchController>(dbContext, logger, configuration)
{
    [HttpGet("projects/{searchText}")]
    [AllowAnonymous]
    [ProjectViewFeature]
    public async Task<ActionResult<List<ProjectSearchResult>>> SearchProjects([FromRoute] string searchText)
    {
        var results = await Projects.SearchForUserAsync(DbContext, searchText, CallingUser);
        return Ok(results);
    }
}
