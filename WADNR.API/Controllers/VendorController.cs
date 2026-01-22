using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("vendors")]
public class VendorController(
    WADNRDbContext dbContext,
    ILogger<VendorController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<VendorController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VendorGridRow>>> List()
    {
        var vendors = await Vendors.ListAsGridRowAsync(DbContext);
        return Ok(vendors);
    }

    [HttpGet("{vendorID}")]
    [EntityNotFound(typeof(Vendor), "vendorID")]
    public async Task<ActionResult<VendorDetail>> Get([FromRoute] int vendorID)
    {
        var vendor = await Vendors.GetByIDAsDetailAsync(DbContext, vendorID);
        if (vendor == null)
        {
            return NotFound();
        }
        return Ok(vendor);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<VendorLookupItem>>> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Ok(new List<VendorLookupItem>());
        }
        var vendors = await Vendors.SearchAsync(DbContext, term);
        return Ok(vendors);
    }

    [HttpGet("{vendorID}/people")]
    [EntityNotFound(typeof(Vendor), "vendorID")]
    public async Task<ActionResult<IEnumerable<VendorPersonGridRow>>> ListPeople([FromRoute] int vendorID)
    {
        var people = await Vendors.ListPeopleAsync(DbContext, vendorID);
        return Ok(people);
    }

    [HttpGet("{vendorID}/organizations")]
    [EntityNotFound(typeof(Vendor), "vendorID")]
    public async Task<ActionResult<IEnumerable<VendorOrganizationGridRow>>> ListOrganizations([FromRoute] int vendorID)
    {
        var organizations = await Vendors.ListOrganizationsAsync(DbContext, vendorID);
        return Ok(organizations);
    }
}
