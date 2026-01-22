using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("taxonomy-trunks")]
public class TaxonomyTrunkController(
    WADNRDbContext dbContext,
    ILogger<TaxonomyTrunkController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<TaxonomyTrunkController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet("lookup")]
    public async Task<ActionResult<IEnumerable<TaxonomyTrunkLookupItem>>> ListAsLookup()
    {
        var items = await TaxonomyTrunks.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }
}
