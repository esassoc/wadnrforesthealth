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
using WADNR.API.ExcelSpecs;
using WADNR.Common.ExcelWorkbookUtilities;
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
    [UserManageFeature]
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
        var gate = await CheckEsaAdminGate(personID);
        if (gate != null) return gate;

        var person = await People.GetByIDAsDetailAsync(DbContext, personID);
        return RequireNotNullThrowNotFound(person, "Person", personID);
    }

    [HttpGet("{personID}/projects")]
    [NormalUserFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<ProjectForPersonDetailGridRow>>> ListProjects([FromRoute] int personID)
    {
        var gate = await CheckEsaAdminGate(personID);
        if (gate != null) return gate;

        var projects = await Projects.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(projects);
    }

    [HttpGet("{personID}/agreements")]
    [NormalUserFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<AgreementGridRow>>> ListAgreements([FromRoute] int personID)
    {
        var gate = await CheckEsaAdminGate(personID);
        if (gate != null) return gate;

        var agreements = await Agreements.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(agreements);
    }

    [HttpGet("{personID}/agreements/excel-download")]
    [ExcelDownloadFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<IActionResult> AgreementsExcelDownload([FromRoute] int personID)
    {
        var gate = await CheckEsaAdminGate(personID);
        if (gate != null) return gate;

        var agreements = await Agreements.ListForPersonAsExcelRowAsync(DbContext, personID);
        var spec = new AgreementExcelSpec();
        var sheet = ExcelWorkbookSheetDescriptorFactory.MakeWorksheet("Agreements", spec, agreements);
        return ExcelFileResult(new ExcelWorkbookMaker(sheet), "Agreements.xlsx");
    }

    [HttpGet("{personID}/interaction-events")]
    [NormalUserFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<InteractionEventGridRow>>> ListInteractionEvents([FromRoute] int personID)
    {
        var gate = await CheckEsaAdminGate(personID);
        if (gate != null) return gate;

        var events = await InteractionEvents.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(events);
    }

    [HttpPut("{personID}/primary-contact-organizations")]
    [UserManageFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> UpdatePrimaryContactOrganizations([FromRoute] int personID, [FromBody] PersonPrimaryContactOrganizationsUpdateRequest dto)
    {
        var updated = await People.UpdatePrimaryContactOrganizationsAsync(DbContext, personID, dto);
        return RequireNotNullThrowNotFound(updated, "Person", personID);
    }

    [HttpPost("contacts")]
    [UserManageFeature]
    public async Task<ActionResult<PersonDetail>> CreateContact([FromBody] ContactUpsertRequest request)
    {
        var created = await People.CreateContactAsync(DbContext, request, CallingUser.PersonID);
        return Ok(created);
    }

    [HttpPut("{personID}")]
    [PersonEditFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> UpdateContact([FromRoute] int personID, [FromBody] PersonUpsertRequest request)
    {
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
        return RequireNotNullThrowNotFound(updated, "Person", personID);
    }

    [HttpPut("{personID}/roles")]
    [UserManageFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> UpdateRoles([FromRoute] int personID, [FromBody] PersonRolesUpsertRequest request)
    {
        try
        {
            var updated = await People.UpdateRolesAsync(DbContext, personID, request);
            return RequireNotNullThrowNotFound(updated, "Person", personID);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
    }

    [HttpPut("{personID}/toggle-active")]
    [UserManageFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> ToggleActive([FromRoute] int personID)
    {
        try
        {
            var updated = await People.ToggleActiveAsync(DbContext, personID);
            return RequireNotNullThrowNotFound(updated, "Person", personID);
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
        var gate = await CheckEsaAdminGate(personID);
        if (gate != null) return gate;

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
    [UserManageFeature]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<PersonDetail>> UpdateStewardshipAreas(
        [FromRoute] int personID,
        [FromBody] PersonStewardshipAreasUpsertRequest request)
    {
        var updated = await People.UpdateStewardshipAreasAsync(DbContext, personID, request);
        return RequireNotNullThrowNotFound(updated, "Person", personID);
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

    /// <summary>
    /// Non-EsaAdmin users cannot access data for EsaAdmin-role persons.
    /// Returns Forbid if the gate applies, null otherwise.
    /// </summary>
    private async Task<ActionResult?> CheckEsaAdminGate(int personID)
    {
        var baseRoleIDs = Role.All.Where(r => r.IsBaseRole).Select(r => r.RoleID).ToList();
        var targetRoleID = await DbContext.PersonRoles
            .Where(pr => pr.PersonID == personID && baseRoleIDs.Contains(pr.RoleID))
            .Select(pr => pr.RoleID)
            .FirstOrDefaultAsync();

        if (targetRoleID == (int)RoleEnum.EsaAdmin
            && CallingUser.BaseRole?.RoleID != (int)RoleEnum.EsaAdmin)
        {
            return Forbid();
        }

        return null;
    }
}
