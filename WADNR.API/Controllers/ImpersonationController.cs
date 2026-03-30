using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("impersonation")]
public class ImpersonationController(
    WADNRDbContext dbContext,
    ILogger<ImpersonationController> logger,
    IOptions<WADNRConfiguration> configuration,
    ImpersonationService impersonationService)
    : SitkaController<ImpersonationController>(dbContext, logger, configuration)
{
    [HttpPost("{personID}")]
    [ImpersonateUserFeature]
    public async Task<ActionResult<PersonDetail>> ImpersonateUser([FromRoute] int personID)
    {
        var targetUser = await People.GetByIDAsDetailAsync(DbContext, personID);
        if (targetUser == null)
        {
            return NotFound($"Person with ID {personID} does not exist.");
        }

        if (targetUser.PersonID == CallingUser.PersonID)
        {
            return BadRequest("Cannot impersonate yourself.");
        }

        var impersonatedUser = await impersonationService.ImpersonateUserAsync(HttpContext, personID);
        return Ok(impersonatedUser);
    }

    [HttpPost("stop")]
    [StopImpersonationFeature]
    public async Task<ActionResult<PersonDetail>> StopImpersonation()
    {
        var originalUser = await impersonationService.StopImpersonationAsync(HttpContext);
        return Ok(originalUser);
    }
}
