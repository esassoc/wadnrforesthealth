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
[Route("fund-source-types")]
public class FundSourceTypeController(
    WADNRDbContext dbContext,
    ILogger<FundSourceTypeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FundSourceTypeController>(dbContext, logger, configuration)
{
    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FundSourceTypeLookupItem>>> ListLookup()
    {
        var items = await FundSourceTypes.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }
}
