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
[Route("agreement-statuses")]
public class AgreementStatusController(
    WADNRDbContext dbContext,
    ILogger<AgreementStatusController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<AgreementStatusController>(dbContext, logger, configuration)
{
    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AgreementStatusLookupItem>>> ListLookup()
    {
        var items = await AgreementStatuses.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }
}
