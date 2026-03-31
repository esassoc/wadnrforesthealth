using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.Invoice;

namespace WADNR.Scalar.Controllers;

[ApiController]
[Route("invoices")]
[Authorize]
public class InvoiceController(WADNRDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Returns all invoices, including invoice date, payment amount, approval status, and match amount details.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<InvoiceApiJson>>> List()
    {
        var items = await Invoices.ListAsApiJsonAsync(dbContext);
        return Ok(items);
    }
}
