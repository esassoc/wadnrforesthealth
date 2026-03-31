using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.InvoicePaymentRequest;

namespace WADNR.EFModels.Entities;

public static class InvoicePaymentRequests
{
    public static async Task<List<InvoicePaymentRequestGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.InvoicePaymentRequests
            .AsNoTracking()
            .Where(x => x.ProjectID == projectID)
            .OrderByDescending(x => x.InvoicePaymentRequestDate)
            .Select(InvoicePaymentRequestProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<InvoicePaymentRequest?> GetByIDWithTrackingAsync(WADNRDbContext dbContext, int invoicePaymentRequestID)
    {
        return await dbContext.InvoicePaymentRequests.FindAsync(invoicePaymentRequestID);
    }

    public static async Task<InvoicePaymentRequest> CreateAsync(
        WADNRDbContext dbContext,
        InvoicePaymentRequestUpsertRequest request)
    {
        var entity = new InvoicePaymentRequest
        {
            ProjectID = request.ProjectID,
            VendorID = request.VendorID,
            PreparedByPersonID = request.PreparedByPersonID,
            InvoicePaymentRequestDate = request.InvoicePaymentRequestDate,
            PurchaseAuthority = request.PurchaseAuthority,
            PurchaseAuthorityIsLandownerCostShareAgreement = request.PurchaseAuthorityIsLandownerCostShareAgreement,
            Duns = request.Duns,
            Notes = request.Notes
        };

        dbContext.InvoicePaymentRequests.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }
}
