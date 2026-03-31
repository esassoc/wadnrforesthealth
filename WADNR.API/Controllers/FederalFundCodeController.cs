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
[Route("federal-fund-codes")]
public class FederalFundCodeController(
    WADNRDbContext dbContext,
    ILogger<FederalFundCodeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FederalFundCodeController>(dbContext, logger, configuration)
{
    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FederalFundCodeLookupItem>>> ListLookup()
    {
        var items = await FederalFundCodes.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }
}
