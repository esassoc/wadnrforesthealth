using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FindYourForester;

namespace WADNR.API.Controllers;

[ApiController]
[Route("find-your-forester")]
public class FindYourForesterController(
    WADNRDbContext dbContext,
    ILogger<FindYourForesterController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FindYourForesterController>(dbContext, logger, configuration)
{
    [HttpGet("questions")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FindYourForesterQuestionTreeNode>>> ListQuestions()
    {
        var questions = await FindYourForesterQuestions.ListAsTreeAsync(DbContext);
        return Ok(questions);
    }

    [HttpGet("roles")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ForesterRoleLookupItem>>> ListActiveRoles()
    {
        var roles = await ForesterWorkUnits.ListActiveRolesAsync(DbContext);
        return Ok(roles);
    }

    [HttpGet("work-units/{foresterRoleID}")]
    [FindYourForesterManageFeature]
    public async Task<ActionResult<List<ForesterWorkUnitGridRow>>> ListWorkUnitsForRole([FromRoute] int foresterRoleID)
    {
        var workUnits = await ForesterWorkUnits.ListForRoleAsGridRowAsync(DbContext, foresterRoleID);
        return Ok(workUnits);
    }

    [HttpGet("assignable-people")]
    [FindYourForesterManageFeature]
    public async Task<ActionResult<List<PersonLookupItem>>> ListAssignablePeople()
    {
        var people = await People.ListAsLookupItemAsync(DbContext, wadnrOnly: true);
        return Ok(people);
    }

    [HttpPost("work-units/bulk-assign")]
    [FindYourForesterManageFeature]
    public async Task<IActionResult> BulkAssignForesters([FromBody] BulkAssignForestersRequest request)
    {
        await ForesterWorkUnits.BulkAssignAsync(DbContext, request);
        return Ok();
    }
}
