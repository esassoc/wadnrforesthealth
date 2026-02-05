using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("organizations")]
public class OrganizationController(
    WADNRDbContext dbContext,
    ILogger<OrganizationController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<OrganizationController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationGridRow>>> List()
    {
        var organizations = await Organizations.ListAsGridRowAsync(DbContext);
        return Ok(organizations);
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationLookupItem>>> ListLookup()
    {
        var organizations = await Organizations.ListAsLookupItemAsync(DbContext);
        return Ok(organizations);
    }

    [HttpGet("{organizationID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<OrganizationDetail>> Get([FromRoute] int organizationID)
    {
        var entity = await Organizations.GetByIDAsDetailAsync(DbContext, organizationID);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost]
    [UserManageFeature]
    public async Task<ActionResult<OrganizationDetail>> Create([FromBody] OrganizationUpsertRequest dto)
    {
        var created = await Organizations.CreateAsync(DbContext, dto, CallingUser.PersonID);
        if (created == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { organizationID = created.OrganizationID }, created);
    }

    [HttpPut("{organizationID}")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<OrganizationDetail>> Update([FromRoute] int organizationID, [FromBody] OrganizationUpsertRequest dto)
    {
        var updated = await Organizations.UpdateAsync(DbContext, organizationID, dto, CallingUser.PersonID);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{organizationID}")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<IActionResult> Delete([FromRoute] int organizationID)
    {
        var deleted = await Organizations.DeleteAsync(DbContext, organizationID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{organizationID}/programs")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<IEnumerable<ProgramGridRow>>> ListProgramsForOrganization([FromRoute] int organizationID)
    {
        var programs = await Programs.ListAsGridRowByOrganizationIDAsync(DbContext, organizationID);
        return Ok(programs);
    }

    [HttpGet("{organizationID}/projects")]
    [ProjectViewFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<IEnumerable<ProjectOrganizationDetailGridRow>>> ListProjectsForOrganization([FromRoute] int organizationID)
    {
        var projects = await Projects.ListAsOrganizationDetailGridRowForUserAsync(DbContext, organizationID, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{organizationID}/projects/pending")]
    [ProjectPendingViewFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<IEnumerable<ProjectOrganizationDetailGridRow>>> ListPendingProjectsForOrganization([FromRoute] int organizationID)
    {
        var projects = await Projects.ListPendingAsOrganizationDetailGridRowForUserAsync(DbContext, organizationID, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{organizationID}/agreements")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<IEnumerable<AgreementGridRow>>> ListAgreementsForOrganization([FromRoute] int organizationID)
    {
        var agreements = await Agreements.ListAsGridRowByOrganizationIDAsync(DbContext, organizationID);
        return Ok(agreements);
    }

    [HttpGet("{organizationID}/boundary")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<FeatureCollection>> GetBoundary([FromRoute] int organizationID)
    {
        var features = await Organizations.GetBoundaryAsFeatureCollectionAsync(DbContext, organizationID);
        return Ok(features);
    }

    [HttpDelete("{organizationID}/boundary")]
    [UserManageFeature]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<IActionResult> DeleteBoundary([FromRoute] int organizationID)
    {
        var deleted = await Organizations.DeleteBoundaryAsync(DbContext, organizationID);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{organizationID}/project-locations")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Organization), "organizationID")]
    public async Task<ActionResult<FeatureCollection>> GetProjectLocations([FromRoute] int organizationID)
    {
        var features = await Organizations.GetProjectLocationsAsFeatureCollectionAsync(DbContext, organizationID);
        return Ok(features);
    }
}
