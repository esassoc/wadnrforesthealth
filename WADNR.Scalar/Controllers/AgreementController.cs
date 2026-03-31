using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.Agreement;

namespace WADNR.Scalar.Controllers;

[ApiController]
[Route("agreements")]
[Authorize]
public class AgreementController(WADNRDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Returns all agreements, including agreement type, status, organization, region, dollar amounts, and key dates.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AgreementApiJson>>> List()
    {
        var items = await Agreements.ListAsApiJsonAsync(dbContext);
        return Ok(items);
    }
}
