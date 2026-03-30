using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.FundSource;

namespace WADNR.Scalar.Controllers;

[ApiController]
[Route("fund-sources")]
[Authorize]
public class FundSourceController(WADNRDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Returns all fund sources, including funding type, status, awarding organization, awarded amount, and compliance details.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<FundSourceApiJson>>> List()
    {
        var items = await FundSources.ListAsApiJsonAsync(dbContext);
        return Ok(items);
    }

    /// <summary>
    /// Returns the list of possible fund source statuses (e.g., Active, Closed). Use these values to interpret the status fields on fund source records.
    /// </summary>
    [HttpGet("statuses")]
    public ActionResult<List<FundSourceStatusApiJson>> ListStatuses()
    {
        var items = FundSources.ListStatusesAsApiJson();
        return Ok(items);
    }
}
