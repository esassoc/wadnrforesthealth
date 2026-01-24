using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.API.Controllers;

[ApiController]
[Route("fund-source-allocations")]
public class FundSourceAllocationController(
    WADNRDbContext dbContext,
    ILogger<FundSourceAllocationController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FundSourceAllocationController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationGridRow>>> List()
    {
        var fundSourceAllocations = await FundSourceAllocations.ListAsGridRowAsync(DbContext);
        return Ok(fundSourceAllocations);
    }

    [HttpGet("{fundSourceAllocationID}")]
    [AllowAnonymous]
    public async Task<ActionResult<FundSourceAllocationDetail>> GetByID([FromRoute] int fundSourceAllocationID)
    {
        var fundSourceAllocation = await FundSourceAllocations.GetByIDAsDetailAsync(DbContext, fundSourceAllocationID);
        if (fundSourceAllocation == null)
        {
            return NotFound();
        }
        return Ok(fundSourceAllocation);
    }

    [HttpGet("for-fund-source/{fundSourceID}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationGridRow>>> ListForFundSource([FromRoute] int fundSourceID)
    {
        var fundSourceAllocations = await FundSourceAllocations.ListForFundSourceAsGridRowAsync(DbContext, fundSourceID);
        return Ok(fundSourceAllocations);
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FundSourceAllocationLookupItem>>> ListLookup()
    {
        var fundSourceAllocations = await FundSourceAllocations.ListAsLookupItemAsync(DbContext);
        return Ok(fundSourceAllocations);
    }
}
