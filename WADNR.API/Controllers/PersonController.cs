using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("people")]
public class PersonController(
    WADNRDbContext dbContext,
    ILogger<PersonController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<PersonController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonGridRow>>> List()
    {
        var items = await People.ListAsGridRowAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<IEnumerable<PersonLookupItem>>> ListLookup()
    {
        var items = await People.ListAsLookupItemAsync(DbContext);
        return Ok(items);
    }

    [HttpGet("{personID}")]
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
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> ListProjects([FromRoute] int personID)
    {
        var projects = await Projects.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(projects);
    }

    [HttpGet("{personID}/agreements")]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<AgreementGridRow>>> ListAgreements([FromRoute] int personID)
    {
        var agreements = await Agreements.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(agreements);
    }

    [HttpGet("{personID}/interaction-events")]
    [EntityNotFound(typeof(Person), "personID")]
    public async Task<ActionResult<IEnumerable<InteractionEventGridRow>>> ListInteractionEvents([FromRoute] int personID)
    {
        var events = await InteractionEvents.ListForPersonAsGridRowAsync(DbContext, personID);
        return Ok(events);
    }
}
