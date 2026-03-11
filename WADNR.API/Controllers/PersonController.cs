using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("people")]
public class PersonController(
    WADNRDbContext dbContext,
    ILogger<PersonController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<PersonController>(dbContext, logger, configuration)
{
    [HttpGet]
    [NormalUserFeature]
    public async Task<ActionResult<IEnumerable<PersonGridRow>>> List()
    {
        var items = await People.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("lookup")]
    [NormalUserFeature]
    public async Task<ActionResult<IEnumerable<PersonLookupItem>>> ListLookup()
    {
        var items = await People.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("lookup/wadnr")]
    [NormalUserFeature]
    public async Task<ActionResult<IEnumerable<PersonWithOrganizationLookupItem>>> ListWadnrLookup()
    {
        var items = await People.ListWadnrAsLookupItemAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{personID}")]
    [NormalUserFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> Get([FromRoute] int personID)
    {
        var person = await People.GetByIDAsDetailAsync(DbContext, personID);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }

    [HttpGet("{personID}/projects")]
    [NormalUserFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> ListProjects([FromRoute] int personID)
    {
        var projects = await Projects.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(projects);
    }

    [HttpGet("{personID}/agreements")]
    [NormalUserFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<AgreementGridRow>>> ListAgreements([FromRoute] int personID)
    {
        var agreements = await Agreements.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(agreements);
    }

    [HttpGet("{personID}/interaction-events")]
    [NormalUserFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<InteractionEventGridRow>>> ListInteractionEvents([FromRoute] int personID)
    {
        var events = await InteractionEvents.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(events);
    }

    [HttpPut("{personID}/primary-contact-organizations")]
    [UserManageFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> UpdatePrimaryContactOrganizations([FromRoute] int personID, [FromBody] PersonPrimaryContactOrganizationsUpdateRequest dto)
    {
        var updated = await People.UpdatePrimaryContactOrganizationsAsync(DbContext, personID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpPost("contacts")]
    [UserManageFeature]
    public async Task<ActionResult<PersonDetail>> CreateContact([FromBody] ContactUpsertRequest request)
    {
        var created = await People.CreateContactAsync(DbContext, request, CallingUser.PersonID);
        return Ok(created);
    }

    [HttpPut("{personID}")]
    [UserManageFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> UpdateContact([FromRoute] int personID, [FromBody] PersonUpsertRequestDto request)
    {
        // Allow self-edit or users with manage permission (enforced by [UserManageFeature] for non-self)
        var callingUserID = CallingUser.PersonID;

        // Determine if target person is a full user
        var personRoleIDs = await DbContext.PersonRoles
            .Where(pr => pr.PersonID == personID)
            .Select(pr => pr.RoleID)
            .ToListAsync();

        var roleLookup = Role.AllLookupDictionary;
        var baseRole = personRoleIDs
            .Select(id => roleLookup.TryGetValue(id, out var r) ? r : null)
            .FirstOrDefault(r => r?.IsBaseRole == true);

        var isFullUser = baseRole != null && baseRole != Role.Unassigned;

        var updated = await People.UpdateContactAsync(DbContext, personID, request, isFullUser);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpPut("{personID}/roles")]
    [AdminFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> UpdateRoles([FromRoute] int personID, [FromBody] PersonRolesUpsertRequestDto request)
    {
        try
        {
            var updated = await People.UpdateRolesAsync(DbContext, personID, request);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPut("{personID}/toggle-active")]
    [AdminFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> ToggleActive([FromRoute] int personID)
    {
        try
        {
            var updated = await People.ToggleActiveAsync(DbContext, personID);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpGet("{personID}/notifications")]
    [NormalUserFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<NotificationGridRow>>> ListNotifications([FromRoute] int personID)
    {
        var notifications = await People.ListNotificationsForPersonAsGridRowAsync(DbContext, personID);
        return Ok(notifications);
    }

    [HttpGet("stewardship-areas/regions")]
    [NormalUserFeature]
    public async Task<ActionResult<IEnumerable<StewardshipAreaItem>>> ListStewardshipRegions()
    {
        var regions = await People.ListStewardshipRegionsAsync(DbContext);
        return Ok(regions);
    }

    [HttpPut("{personID}/stewardship-areas")]
    [AdminFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> UpdateStewardshipAreas(
        [FromRoute] int personID,
        [FromBody] PersonStewardshipAreasUpsertRequest request)
    {
        var updated = await People.UpdateStewardshipAreasAsync(DbContext, personID, request);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{personID}")]
    [UserManageFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult> DeleteContact([FromRoute] int personID)
    {
        try
        {
            await People.DeleteContactAsync(DbContext, personID);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }
}
