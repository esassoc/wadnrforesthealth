using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.Invoice;

namespace WADNR.API.Controllers;

[ApiController]
[Route("invoices")]
public class InvoiceController(
    WADNRDbContext dbContext,
    ILogger<InvoiceController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<InvoiceController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<List<InvoiceGridRow>>> List()
    {
        var invoices = await Invoices.ListAsGridRowAsync(DbContext);
        return Ok(invoices);
    }

    [HttpGet("{invoiceID}")]
    public async Task<ActionResult<InvoiceDetail>> GetByID([FromRoute] int invoiceID)
    {
        var invoice = await Invoices.GetByIDAsDetailAsync(DbContext, invoiceID);
        if (invoice == null)
        {
            return NotFound();
        }
        return Ok(invoice);
    }

    [HttpGet("for-project/{projectID}")]
    public async Task<ActionResult<List<InvoiceGridRow>>> ListForProject([FromRoute] int projectID)
    {
        var invoices = await Invoices.ListForProjectAsGridRowAsync(DbContext, projectID);
        return Ok(invoices);
    }

    [HttpGet("for-payment-request/{invoicePaymentRequestID}")]
    public async Task<ActionResult<List<InvoiceGridRow>>> ListForPaymentRequest([FromRoute] int invoicePaymentRequestID)
    {
        var invoices = await Invoices.ListForPaymentRequestAsGridRowAsync(DbContext, invoicePaymentRequestID);
        return Ok(invoices);
    }
}
