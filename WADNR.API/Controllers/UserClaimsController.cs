using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
[Route("user-claims")]
public class UserClaimsController(
    WADNRDbContext dbContext,
    ILogger<UserClaimsController> logger,
    IOptions<WADNRConfiguration> neptuneConfiguration)
    : SitkaController<UserClaimsController>(dbContext, logger, neptuneConfiguration)
{
    [HttpGet("{globalID}")]
    [LoggedInFeature]
    public async Task<ActionResult<PersonDetail>> GetByGlobalID([FromRoute] string globalID)
    {
        var userDto = await People.GetByGlobalIDAsDetailAsync(DbContext, globalID);
        if (userDto == null)
        {
            var notFoundMessage = $"User with GlobalID {globalID} does not exist!";
            Logger.LogError(notFoundMessage);
            return NotFound(notFoundMessage);
        }

        return Ok(userDto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PersonDetail>> PostUserClaims([FromServices] HttpContext httpContext)
    {
        var claimsPrincipal = httpContext.User;
        if (!claimsPrincipal.Claims.Any())  // Updating user based on claims does not work when there are no claims
        {
            return BadRequest();
        }

        var updatedUserDto = await People.UpdateClaims(dbContext, claimsPrincipal);
        return Ok(updatedUserDto);
    }
}