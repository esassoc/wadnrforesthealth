using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("funding-sources")]
public class FundingSourceController(
    WADNRDbContext dbContext,
    ILogger<FundingSourceController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FundingSourceController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<FundingSourceLookupItem>> List()
    {
        var items = FundingSource.All
            .OrderBy(x => x.FundingSourceID)
            .Select(x => new FundingSourceLookupItem
            {
                FundingSourceID = x.FundingSourceID,
                FundingSourceName = x.FundingSourceDisplayName
            })
            .ToList();
        return Ok(items);
    }
}
