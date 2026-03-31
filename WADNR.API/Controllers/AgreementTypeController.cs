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
[Route("agreement-types")]
public class AgreementTypeController(
    WADNRDbContext dbContext,
    ILogger<AgreementTypeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<AgreementTypeController>(dbContext, logger, configuration)
{
    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AgreementTypeLookupItem>>> ListLookup()
    {
        var items = await AgreementTypes.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }
}
