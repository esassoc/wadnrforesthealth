using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class VendorProjections
{
    public static readonly Expression<Func<Vendor, VendorGridRow>> AsGridRow = x => new VendorGridRow
    {
        VendorID = x.VendorID,
        VendorName = x.VendorName,
        StatewideVendorNumber = x.StatewideVendorNumber,
        StatewideVendorNumberSuffix = x.StatewideVendorNumberSuffix,
        BillingAgency = x.BillingAgency,
        BillingSubAgency = x.BillingSubAgency,
        BillingFund = x.BillingFund,
        BillingFundBreakout = x.BillingFundBreakout,
        VendorAddressLine1 = x.VendorAddressLine1,
        VendorAddressLine2 = x.VendorAddressLine2,
        VendorAddressLine3 = x.VendorAddressLine3,
        VendorCity = x.VendorCity,
        VendorState = x.VendorState,
        VendorZip = x.VendorZip,
        Remarks = x.Remarks,
        VendorPhone = x.VendorPhone,
        VendorStatus = x.VendorStatus,
        TaxpayerIdNumber = x.TaxpayerIdNumber,
        Email = x.Email
    };

    public static readonly Expression<Func<Vendor, VendorExcelRow>> AsExcelRow = x => new VendorExcelRow
    {
        VendorID = x.VendorID,
        VendorType = x.VendorType,
        BillingAgency = x.BillingAgency,
        BillingSubAgency = x.BillingSubAgency,
        BillingFund = x.BillingFund,
        BillingFundBreakout = x.BillingFundBreakout,
        VendorAddressLine1 = x.VendorAddressLine1,
        VendorAddressLine2 = x.VendorAddressLine2,
        VendorAddressLine3 = x.VendorAddressLine3,
        VendorCity = x.VendorCity,
        VendorState = x.VendorState,
        VendorZip = x.VendorZip,
        Remarks = x.Remarks,
        VendorPhone = x.VendorPhone,
        VendorStatus = x.VendorStatus,
        TaxpayerIdNumber = x.TaxpayerIdNumber,
        Email = x.Email
    };

    public static readonly Expression<Func<Vendor, VendorDetail>> AsDetail = x => new VendorDetail
    {
        VendorID = x.VendorID,
        VendorName = x.VendorName,
        StatewideVendorNumber = x.StatewideVendorNumber,
        StatewideVendorNumberSuffix = x.StatewideVendorNumberSuffix,
        VendorType = x.VendorType,
        VendorStatus = x.VendorStatus,

        // Billing Info
        BillingAgency = x.BillingAgency,
        BillingSubAgency = x.BillingSubAgency,
        BillingFund = x.BillingFund,
        BillingFundBreakout = x.BillingFundBreakout,

        // Address
        VendorAddressLine1 = x.VendorAddressLine1,
        VendorAddressLine2 = x.VendorAddressLine2,
        VendorAddressLine3 = x.VendorAddressLine3,
        VendorCity = x.VendorCity,
        VendorState = x.VendorState,
        VendorZip = x.VendorZip,

        // Contact
        VendorPhone = x.VendorPhone,
        Email = x.Email,

        // Other
        Remarks = x.Remarks,
        TaxpayerIdNumber = x.TaxpayerIdNumber,

        // Counts
        PersonCount = x.People.Count(p => p.IsActive),
        OrganizationCount = x.Organizations.Count(o => o.IsActive)
    };

    public static readonly Expression<Func<Vendor, VendorLookupItem>> AsLookupItem = x => new VendorLookupItem
    {
        VendorID = x.VendorID,
        VendorName = x.VendorName,
        StatewideVendorNumber = x.StatewideVendorNumber,
        StatewideVendorNumberSuffix = x.StatewideVendorNumberSuffix,
        DisplayName = x.VendorName + " (" + x.StatewideVendorNumber + "-" + x.StatewideVendorNumberSuffix + ")"
    };
}
