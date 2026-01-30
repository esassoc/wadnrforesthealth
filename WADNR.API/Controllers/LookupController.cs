using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

/// <summary>
/// Controller for various lookup/reference data endpoints.
/// These are used to populate dropdowns and don't require project-specific context.
/// </summary>
[ApiController]
[Route("lookups")]
public class LookupController(
    WADNRDbContext dbContext,
    ILogger<LookupController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<LookupController>(dbContext, logger, configuration)
{
    /// <summary>
    /// Get all person relationship types for project contacts.
    /// </summary>
    [HttpGet("person-relationship-types")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<PersonRelationshipTypeLookupItem>> ListPersonRelationshipTypes()
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

    /// <summary>
    /// Get all organization relationship types.
    /// </summary>
    [HttpGet("organization-relationship-types")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<OrganizationRelationshipTypeLookupItem>>> ListOrganizationRelationshipTypes()
    {
        var items = await DbContext.RelationshipTypes
            .AsNoTracking()
            .OrderBy(x => x.IsPrimaryContact ? 0 : 1)
            .ThenBy(x => x.RelationshipTypeName)
            .Select(x => new OrganizationRelationshipTypeLookupItem
            {
                RelationshipTypeID = x.RelationshipTypeID,
                RelationshipTypeName = x.RelationshipTypeName,
                RelationshipTypeDescription = x.RelationshipTypeDescription,
                CanOnlyBeRelatedOnceToAProject = x.CanOnlyBeRelatedOnceToAProject,
                IsPrimaryContact = x.IsPrimaryContact,
                SortOrder = x.IsPrimaryContact ? 0 : 1
            })
            .ToListAsync();
        return Ok(items);
    }

    /// <summary>
    /// Get all funding sources (Federal, State, Private, Other).
    /// </summary>
    [HttpGet("funding-sources")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<FundingSourceOption>> ListFundingSources()
    {
        var items = FundingSource.All
            .OrderBy(x => x.FundingSourceID)
            .Select(x => new FundingSourceOption
            {
                FundingSourceID = x.FundingSourceID,
                FundingSourceName = x.FundingSourceDisplayName
            })
            .ToList();
        return Ok(items);
    }

    /// <summary>
    /// Get all classification systems with their classifications.
    /// </summary>
    [HttpGet("classification-systems-with-classifications")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClassificationSystemWithClassifications>>> ListClassificationSystemsWithClassifications()
    {
        var items = await ClassificationSystems.ListWithClassificationsAsync(DbContext);
        return Ok(items);
    }
}
