using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.Invoice;

namespace WADNR.EFModels.Entities;

public static class Invoices
{
    public static async Task<List<InvoiceGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var invoices = await dbContext.Invoices
            .AsNoTracking()
            .Select(InvoiceProjections.AsGridRow)
            .ToListAsync();

        // Map static enum values
        foreach (var invoice in invoices)
        {
            MapStaticEnumValues(invoice);
        }

        return invoices;
    }

    public static async Task<List<InvoiceGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var invoices = await dbContext.Invoices
            .AsNoTracking()
            .Where(x => x.InvoicePaymentRequest.ProjectID == projectID)
            .Select(InvoiceProjections.AsGridRow)
            .ToListAsync();

        // Map static enum values
        foreach (var invoice in invoices)
        {
            MapStaticEnumValues(invoice);
        }

        return invoices;
    }

    public static async Task<List<InvoiceGridRow>> ListForPaymentRequestAsGridRowAsync(WADNRDbContext dbContext, int invoicePaymentRequestID)
    {
        var invoices = await dbContext.Invoices
            .AsNoTracking()
            .Where(x => x.InvoicePaymentRequestID == invoicePaymentRequestID)
            .Select(InvoiceProjections.AsGridRow)
            .ToListAsync();

        // Map static enum values
        foreach (var invoice in invoices)
        {
            MapStaticEnumValues(invoice);
        }

        return invoices;
    }

    public static async Task<InvoiceDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int invoiceID)
    {
        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .Where(x => x.InvoiceID == invoiceID)
            .Select(InvoiceProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (invoice != null)
        {
            MapStaticEnumValuesForDetail(invoice);
        }

        return invoice;
    }

    private static void MapStaticEnumValues(InvoiceGridRow invoice)
    {
        // Map InvoiceStatus
        if (InvoiceStatus.AllLookupDictionary.TryGetValue(invoice.InvoiceStatusID, out var status))
        {
            invoice.InvoiceStatusDisplayName = status.InvoiceStatusDisplayName;
        }

        // Map OrganizationCode
        if (invoice.OrganizationCodeID.HasValue &&
            OrganizationCode.AllLookupDictionary.TryGetValue(invoice.OrganizationCodeID.Value, out var orgCode))
        {
            invoice.OrganizationCodeName = orgCode.OrganizationCodeName;
        }
    }

    private static void MapStaticEnumValuesForDetail(InvoiceDetail invoice)
    {
        // Map InvoiceStatus
        if (InvoiceStatus.AllLookupDictionary.TryGetValue(invoice.InvoiceStatusID, out var status))
        {
            invoice.InvoiceStatusDisplayName = status.InvoiceStatusDisplayName;
        }

        // Map InvoiceMatchAmountType
        if (InvoiceMatchAmountType.AllLookupDictionary.TryGetValue(invoice.InvoiceMatchAmountTypeID, out var matchType))
        {
            invoice.InvoiceMatchAmountTypeDisplayName = matchType.InvoiceMatchAmountTypeDisplayName;
        }

        // Map OrganizationCode
        if (invoice.OrganizationCodeID.HasValue &&
            OrganizationCode.AllLookupDictionary.TryGetValue(invoice.OrganizationCodeID.Value, out var orgCode))
        {
            invoice.OrganizationCodeName = orgCode.OrganizationCodeName;
            invoice.OrganizationCodeValue = orgCode.OrganizationCodeValue;
        }
    }
}
