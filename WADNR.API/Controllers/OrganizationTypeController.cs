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
[Route("organization-types")]
public class OrganizationTypeController(
    WADNRDbContext dbContext,
    ILogger<OrganizationTypeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<OrganizationTypeController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationTypeLookupItem>>> List()
    {
        var items = await OrganizationTypes.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }
}
