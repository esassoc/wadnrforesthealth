using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.InvoicePaymentRequest;

namespace WADNR.EFModels.Entities;

public static class InvoicePaymentRequestProjections
{
    public static Expression<Func<InvoicePaymentRequest, InvoicePaymentRequestGridRow>> AsGridRow => x => new InvoicePaymentRequestGridRow
    {
        InvoicePaymentRequestID = x.InvoicePaymentRequestID,
        ProjectID = x.ProjectID,
        InvoicePaymentRequestDate = x.InvoicePaymentRequestDate,
        VendorID = x.VendorID,
        VendorName = x.Vendor != null ? x.Vendor.VendorName : null,
        VendorAddress = x.Vendor != null
            ? (x.Vendor.VendorAddressLine1 ?? "")
              + (x.Vendor.VendorCity != null ? " " + x.Vendor.VendorCity : "")
              + (x.Vendor.VendorState != null ? " " + x.Vendor.VendorState : "")
              + (x.Vendor.VendorZip != null ? " " + x.Vendor.VendorZip : "")
            : null,
        VendorStatewideVendorNumber = x.Vendor != null
            ? x.Vendor.StatewideVendorNumber + "-" + x.Vendor.StatewideVendorNumberSuffix
            : null,
        PreparedByPersonID = x.PreparedByPersonID,
        PreparedByPersonFullName = x.PreparedByPerson != null
            ? x.PreparedByPerson.FirstName + " " + x.PreparedByPerson.LastName
            : null,
        PreparedByPersonPhone = x.PreparedByPerson != null ? x.PreparedByPerson.Phone : null,
        PurchaseAuthority = x.PurchaseAuthority,
        PurchaseAuthorityIsLandownerCostShareAgreement = x.PurchaseAuthorityIsLandownerCostShareAgreement,
        Duns = x.Duns,
        Notes = x.Notes,
        InvoiceCount = x.Invoices.Count
    };
}
