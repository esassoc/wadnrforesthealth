using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.Invoice;
using WADNR.Models.DataTransferObjects.InvoicePaymentRequest;

namespace WADNR.API.Controllers;

[ApiController]
[Route("invoice-payment-requests")]
public class InvoicePaymentRequestController(
    WADNRDbContext dbContext,
    ILogger<InvoicePaymentRequestController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<InvoicePaymentRequestController>(dbContext, logger, configuration)
{
    [HttpGet("{invoicePaymentRequestID}/invoices")]
    [AllowAnonymous]
    public async Task<ActionResult<List<InvoiceGridRow>>> ListInvoices([FromRoute] int invoicePaymentRequestID)
    {
        var invoices = await Invoices.ListForPaymentRequestAsGridRowAsync(DbContext, invoicePaymentRequestID);
        return Ok(invoices);
    }

    [HttpPost]
    [ProjectEditAsAdminFeature]
    public async Task<ActionResult<InvoicePaymentRequestGridRow>> Create([FromBody] InvoicePaymentRequestUpsertRequest request)
    {
        if (request.PreparedByPersonID == null)
        {
            return BadRequest("Prepared By is required.");
        }

        var project = await DbContext.Projects.FindAsync(request.ProjectID);
        if (project == null)
        {
            return NotFound($"Project with ID {request.ProjectID} not found.");
        }

        var entity = await InvoicePaymentRequests.CreateAsync(DbContext, request);

        var rows = await InvoicePaymentRequests.ListForProjectAsGridRowAsync(DbContext, request.ProjectID);
        var row = rows.FirstOrDefault(x => x.InvoicePaymentRequestID == entity.InvoicePaymentRequestID);

        return Ok(row);
    }
}
