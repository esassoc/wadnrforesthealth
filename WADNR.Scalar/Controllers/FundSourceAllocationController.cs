using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.Scalar.Controllers;

[ApiController]
[Route("fund-source-allocations")]
[Authorize]
public class FundSourceAllocationController(WADNRDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Returns all fund source allocations, including allocation amount, region, division, federal fund code, managing organization, and fund source manager.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<FundSourceAllocationApiJson>>> List()
    {
        var items = await FundSourceAllocations.ListAsApiJsonAsync(dbContext);
        return Ok(items);
    }

    /// <summary>
    /// Returns the program index and project code pairings assigned to each fund source allocation, used for state budget tracking.
    /// </summary>
    [HttpGet("program-index-project-codes")]
    public async Task<ActionResult<List<FundSourceAllocationProgramIndexProjectCodeApiJson>>> ListProgramIndexProjectCodes()
    {
        var items = await FundSourceAllocations.ListProgramIndexProjectCodesAsApiJsonAsync(dbContext);
        return Ok(items);
    }
}
