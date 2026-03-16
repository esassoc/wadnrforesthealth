using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.Invoice;

namespace WADNR.API.Controllers;

[ApiController]
[Route("invoices")]
public class InvoiceController(
    WADNRDbContext dbContext,
    ILogger<InvoiceController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<InvoiceController>(dbContext, logger, configuration)
{
    [HttpGet]
    [InvoiceManageFeature]
    public async Task<ActionResult<List<InvoiceGridRow>>> List()
    {
        var invoices = await Invoices.ListAsGridRowAsync(DbContext);
        return Ok(invoices);
    }

    [HttpGet("{invoiceID}")]
    [AllowAnonymous]
    public async Task<ActionResult<InvoiceDetail>> GetByID([FromRoute] int invoiceID)
    {
        var invoice = await Invoices.GetByIDAsDetailAsync(DbContext, invoiceID);
        return RequireNotNullThrowNotFound(invoice, "Invoice", invoiceID);
    }

    [HttpPost]
    [InvoiceManageFeature]
    public async Task<ActionResult<InvoiceDetail>> Create([FromBody] InvoiceUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.InvoiceNumber))
        {
            return BadRequest("Invoice Number is required.");
        }

        var paymentRequest = await InvoicePaymentRequests.GetByIDWithTrackingAsync(DbContext, request.InvoicePaymentRequestID);
        if (paymentRequest == null)
        {
            return NotFound($"Invoice Payment Request with ID {request.InvoicePaymentRequestID} not found.");
        }

        var entity = await Invoices.CreateAsync(DbContext, request);
        var detail = await Invoices.GetByIDAsDetailAsync(DbContext, entity.InvoiceID);
        return Ok(detail);
    }

    [HttpPut("{invoiceID}")]
    [InvoiceManageFeature]
    public async Task<ActionResult<InvoiceDetail>> Update([FromRoute] int invoiceID, [FromBody] InvoiceUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.InvoiceNumber))
        {
            return BadRequest("Invoice Number is required.");
        }

        var existingInvoice = await Invoices.GetByIDWithTrackingAsync(DbContext, invoiceID);
        if (existingInvoice == null)
        {
            return NotFound($"Invoice with ID {invoiceID} not found.");
        }

        var entity = await Invoices.UpdateAsync(DbContext, invoiceID, request);
        var detail = await Invoices.GetByIDAsDetailAsync(DbContext, entity.InvoiceID);
        return Ok(detail);
    }

    [HttpPost("{invoiceID}/voucher")]
    [InvoiceManageFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<InvoiceDetail>> UploadVoucher([FromRoute] int invoiceID, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var invoice = await Invoices.GetByIDWithFileResourceAsync(DbContext, invoiceID);
        if (invoice == null)
        {
            return NotFound($"Invoice with ID {invoiceID} not found.");
        }

        // Delete old file if present
        if (invoice.InvoiceFileResource != null)
        {
            var oldGuid = invoice.InvoiceFileResource.FileResourceGUID.ToString();
            invoice.InvoiceFileResourceID = null;
            await DbContext.SaveChangesAsync();
            DbContext.FileResources.Remove(invoice.InvoiceFileResource);
            await DbContext.SaveChangesAsync();
            await fileService.DeleteFileStreamFromBlobStorageAsync(oldGuid);
        }

        // Create new file resource
        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);
        invoice.InvoiceFileResourceID = fileResource.FileResourceID;
        await DbContext.SaveChangesAsync();

        var detail = await Invoices.GetByIDAsDetailAsync(DbContext, invoiceID);
        return Ok(detail);
    }

    [HttpDelete("{invoiceID}/voucher")]
    [InvoiceDeleteFeature]
    public async Task<ActionResult<InvoiceDetail>> DeleteVoucher([FromRoute] int invoiceID)
    {
        var invoice = await Invoices.GetByIDWithFileResourceAsync(DbContext, invoiceID);
        if (invoice == null)
        {
            return NotFound($"Invoice with ID {invoiceID} not found.");
        }

        if (invoice.InvoiceFileResource == null)
        {
            return BadRequest("Invoice does not have a voucher file.");
        }

        var oldGuid = invoice.InvoiceFileResource.FileResourceGUID.ToString();
        invoice.InvoiceFileResourceID = null;
        await DbContext.SaveChangesAsync();
        DbContext.FileResources.Remove(invoice.InvoiceFileResource);
        await DbContext.SaveChangesAsync();
        await fileService.DeleteFileStreamFromBlobStorageAsync(oldGuid);

        var detail = await Invoices.GetByIDAsDetailAsync(DbContext, invoiceID);
        return Ok(detail);
    }

    [HttpGet("approval-statuses")]
    [AllowAnonymous]
    public async Task<ActionResult<List<InvoiceApprovalStatusLookupItem>>> ListApprovalStatuses()
    {
        var statuses = await Invoices.ListApprovalStatusesAsync(DbContext);
        return Ok(statuses);
    }
}
