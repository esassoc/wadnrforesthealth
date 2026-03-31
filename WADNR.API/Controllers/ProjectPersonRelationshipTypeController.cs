using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("project-person-relationship-types")]
public class ProjectPersonRelationshipTypeController(
    WADNRDbContext dbContext,
    ILogger<ProjectPersonRelationshipTypeController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectPersonRelationshipTypeController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<PersonRelationshipTypeLookupItem>> List()
    {
        var items = ProjectPersonRelationshipType.All
            .OrderBy(x => x.SortOrder)
            .Select(x => new PersonRelationshipTypeLookupItem
            {
                ProjectPersonRelationshipTypeID = x.ProjectPersonRelationshipTypeID,
                ProjectPersonRelationshipTypeName = x.ProjectPersonRelationshipTypeDisplayName,
                IsRequired = x.IsRequired
            })
            .ToList();
        return Ok(items);
    }
}
