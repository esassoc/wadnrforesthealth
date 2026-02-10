using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.Invoice;
using InvoiceEntity = WADNR.EFModels.Entities.Invoice;

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

    public static async Task<InvoiceEntity> CreateAsync(WADNRDbContext dbContext, InvoiceUpsertRequest request)
    {
        var entity = new InvoiceEntity
        {
            InvoicePaymentRequestID = request.InvoicePaymentRequestID,
            InvoiceNumber = request.InvoiceNumber,
            InvoiceIdentifyingName = request.InvoiceIdentifyingName,
            InvoiceDate = request.InvoiceDate,
            PaymentAmount = request.PaymentAmount,
            MatchAmount = request.MatchAmount,
            InvoiceMatchAmountTypeID = request.InvoiceMatchAmountTypeID,
            FundSourceID = request.FundSourceID,
            Fund = request.Fund,
            Appn = request.Appn,
            SubObject = request.SubObject,
            ProgramIndexID = request.ProgramIndexID,
            ProjectCodeID = request.ProjectCodeID,
            OrganizationCodeID = request.OrganizationCodeID,
            InvoiceStatusID = request.InvoiceStatusID,
            InvoiceApprovalStatusID = request.InvoiceApprovalStatusID,
            InvoiceApprovalStatusComment = request.InvoiceApprovalStatusComment
        };

        dbContext.Invoices.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public static async Task<InvoiceEntity> UpdateAsync(WADNRDbContext dbContext, int invoiceID, InvoiceUpsertRequest request)
    {
        var entity = await dbContext.Invoices.FindAsync(invoiceID);
        if (entity == null)
        {
            throw new InvalidOperationException($"Invoice with ID {invoiceID} not found.");
        }

        entity.InvoiceNumber = request.InvoiceNumber;
        entity.InvoiceIdentifyingName = request.InvoiceIdentifyingName;
        entity.InvoiceDate = request.InvoiceDate;
        entity.PaymentAmount = request.PaymentAmount;
        entity.MatchAmount = request.MatchAmount;
        entity.InvoiceMatchAmountTypeID = request.InvoiceMatchAmountTypeID;
        entity.FundSourceID = request.FundSourceID;
        entity.Fund = request.Fund;
        entity.Appn = request.Appn;
        entity.SubObject = request.SubObject;
        entity.ProgramIndexID = request.ProgramIndexID;
        entity.ProjectCodeID = request.ProjectCodeID;
        entity.OrganizationCodeID = request.OrganizationCodeID;
        entity.InvoiceStatusID = request.InvoiceStatusID;
        entity.InvoiceApprovalStatusID = request.InvoiceApprovalStatusID;
        entity.InvoiceApprovalStatusComment = request.InvoiceApprovalStatusComment;

        await dbContext.SaveChangesAsync();
        return entity;
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
