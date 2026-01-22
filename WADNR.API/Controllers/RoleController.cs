using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("roles")]
public class RoleController(
    WADNRDbContext dbContext,
    ILogger<RoleController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<RoleController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<List<RoleGridRow>>> List()
    {
        var roles = await Roles.ListAsGridRowAsync(DbContext);
        return Ok(roles);
    }

    [HttpGet("{roleID}")]
    public async Task<ActionResult<RoleDetail>> GetByID([FromRoute] int roleID)
    {
        var role = await Roles.GetByIDAsDetailAsync(DbContext, roleID);
        if (role == null)
        {
            return NotFound();
        }
        return Ok(role);
    }

    [HttpGet("{roleID}/people")]
    public async Task<ActionResult<List<PersonLookupItem>>> ListPeople([FromRoute] int roleID)
    {
        if (!Role.AllLookupDictionary.ContainsKey(roleID))
        {
            return NotFound();
        }
        var people = await Roles.ListPeopleForRoleAsync(DbContext, roleID);
        return Ok(people);
    }
}
