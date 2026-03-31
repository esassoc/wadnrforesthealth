using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.InteractionEvent;

namespace WADNR.Scalar.Controllers;

[ApiController]
[Route("interaction-events")]
[Authorize]
public class InteractionEventController(WADNRDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Returns all interaction events (meetings, site visits, outreach, etc.), including event type, date, staff contact, description, and location geometry.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<InteractionEventApiJson>>> List()
    {
        var items = await InteractionEvents.ListAsApiJsonAsync(dbContext);
        return Ok(items);
    }
}
