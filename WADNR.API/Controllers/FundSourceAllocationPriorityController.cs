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
[Route("fund-source-allocation-priorities")]
public class FundSourceAllocationPriorityController(
    WADNRDbContext dbContext,
    ILogger<FundSourceAllocationPriorityController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FundSourceAllocationPriorityController>(dbContext, logger, configuration)
{
    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FundSourceAllocationPriorityLookupItem>>> ListLookup()
    {
        var items = await FundSourceAllocationPriorities.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }
}
