using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.Invoice;

namespace WADNR.EFModels.Entities;

public static class InvoiceProjections
{
    public static Expression<Func<Invoice, InvoiceGridRow>> AsGridRow => x => new InvoiceGridRow
    {
        InvoiceID = x.InvoiceID,
        InvoicePaymentRequestID = x.InvoicePaymentRequestID,
        ProjectID = x.InvoicePaymentRequest.ProjectID,
        ProjectName = x.InvoicePaymentRequest.Project.ProjectName,
        FundSourceID = x.FundSourceID,
        FundSourceNumber = x.FundSource != null ? x.FundSource.FundSourceNumber : null,
        InvoiceNumber = x.InvoiceNumber,
        InvoiceDate = x.InvoiceDate,
        Fund = x.Fund,
        Appn = x.Appn,
        ProgramIndexCode = x.ProgramIndex != null ? x.ProgramIndex.ProgramIndexCode : null,
        ProjectCodeName = x.ProjectCode != null ? x.ProjectCode.ProjectCodeName : null,
        SubObject = x.SubObject,
        OrganizationCodeID = x.OrganizationCodeID,
        // OrganizationCode is a static enum - we'll map it in the static helper
        OrganizationCodeName = null,
        MatchAmount = x.MatchAmount,
        PaymentAmount = x.PaymentAmount,
        InvoiceStatusID = x.InvoiceStatusID,
        // InvoiceStatus is a static enum - we'll map it in the static helper
        InvoiceStatusDisplayName = string.Empty,
        InvoiceApprovalStatusID = x.InvoiceApprovalStatusID,
        InvoiceApprovalStatusName = x.InvoiceApprovalStatus.InvoiceApprovalStatusName,
        InvoiceIdentifyingName = x.InvoiceIdentifyingName,
        InvoiceFileResourceGuid = x.InvoiceFileResource != null ? x.InvoiceFileResource.FileResourceGUID : null
    };

    public static Expression<Func<Invoice, InvoiceDetail>> AsDetail => x => new InvoiceDetail
    {
        InvoiceID = x.InvoiceID,
        InvoiceNumber = x.InvoiceNumber,
        InvoiceIdentifyingName = x.InvoiceIdentifyingName,
        InvoiceDate = x.InvoiceDate,

        // Payment Request info
        InvoicePaymentRequestID = x.InvoicePaymentRequestID,
        InvoicePaymentRequestDate = x.InvoicePaymentRequest.InvoicePaymentRequestDate,
        PurchaseAuthority = x.InvoicePaymentRequest.PurchaseAuthority,
        PurchaseAuthorityIsLandownerCostShareAgreement = x.InvoicePaymentRequest.PurchaseAuthorityIsLandownerCostShareAgreement,
        Duns = x.InvoicePaymentRequest.Duns,
        PaymentRequestNotes = x.InvoicePaymentRequest.Notes,

        // Project info
        ProjectID = x.InvoicePaymentRequest.ProjectID,
        ProjectName = x.InvoicePaymentRequest.Project.ProjectName,

        // Vendor info
        VendorID = x.InvoicePaymentRequest.VendorID,
        VendorName = x.InvoicePaymentRequest.Vendor != null ? x.InvoicePaymentRequest.Vendor.VendorName : null,

        // Prepared By info
        PreparedByPersonID = x.InvoicePaymentRequest.PreparedByPersonID,
        PreparedByPersonName = x.InvoicePaymentRequest.PreparedByPerson != null
            ? x.InvoicePaymentRequest.PreparedByPerson.FirstName + " " + x.InvoicePaymentRequest.PreparedByPerson.LastName
            : null,

        // Fund Source info
        FundSourceID = x.FundSourceID,
        FundSourceNumber = x.FundSource != null ? x.FundSource.FundSourceNumber : null,
        FundSourceName = x.FundSource != null ? x.FundSource.FundSourceName : null,

        // Financial codes
        Fund = x.Fund,
        Appn = x.Appn,
        SubObject = x.SubObject,

        // Program Index info
        ProgramIndexID = x.ProgramIndexID,
        ProgramIndexCode = x.ProgramIndex != null ? x.ProgramIndex.ProgramIndexCode : null,
        ProgramIndexTitle = x.ProgramIndex != null ? x.ProgramIndex.ProgramIndexTitle : null,

        // Project Code info
        ProjectCodeID = x.ProjectCodeID,
        ProjectCodeName = x.ProjectCode != null ? x.ProjectCode.ProjectCodeName : null,
        ProjectCodeTitle = x.ProjectCode != null ? x.ProjectCode.ProjectCodeTitle : null,

        // Organization Code - static enum, will be mapped in static helper
        OrganizationCodeID = x.OrganizationCodeID,
        OrganizationCodeName = null,
        OrganizationCodeValue = null,

        // Amounts
        PaymentAmount = x.PaymentAmount,
        MatchAmount = x.MatchAmount,

        // Match Amount Type - static enum, will be mapped in static helper
        InvoiceMatchAmountTypeID = x.InvoiceMatchAmountTypeID,
        InvoiceMatchAmountTypeDisplayName = string.Empty,

        // Status - static enum, will be mapped in static helper
        InvoiceStatusID = x.InvoiceStatusID,
        InvoiceStatusDisplayName = string.Empty,

        // Approval Status
        InvoiceApprovalStatusID = x.InvoiceApprovalStatusID,
        InvoiceApprovalStatusName = x.InvoiceApprovalStatus.InvoiceApprovalStatusName,
        InvoiceApprovalStatusComment = x.InvoiceApprovalStatusComment,

        // Invoice Voucher File
        InvoiceFileResourceID = x.InvoiceFileResourceID,
        InvoiceFileResourceGuid = x.InvoiceFileResource != null ? x.InvoiceFileResource.FileResourceGUID : null,
        InvoiceFileOriginalFileName = x.InvoiceFileResource != null
            ? x.InvoiceFileResource.OriginalBaseFilename + "." + x.InvoiceFileResource.OriginalFileExtension
            : null
    };
}
