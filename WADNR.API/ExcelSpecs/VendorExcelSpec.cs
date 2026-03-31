using WADNR.Common;
using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.ExcelSpecs;

public class VendorExcelSpec : ExcelWorksheetSpec<VendorExcelRow>
{
    public VendorExcelSpec()
    {
        AddColumn("Vendor ID", x => x.VendorID);
        AddColumn("Vendor Type", x => x.VendorType ?? string.Empty);
        AddColumn("Billing Agency", x => x.BillingAgency ?? string.Empty);
        AddColumn("Billing Sub Agency", x => x.BillingSubAgency ?? string.Empty);
        AddColumn("Billing Fund", x => x.BillingFund ?? string.Empty);
        AddColumn("Billing Fund Breakout", x => x.BillingFundBreakout ?? string.Empty);
        AddColumn("Vendor Address Line 1", x => x.VendorAddressLine1 ?? string.Empty);
        AddColumn("Vendor Address Line 2", x => x.VendorAddressLine2 ?? string.Empty);
        AddColumn("Vendor Address Line 3", x => x.VendorAddressLine3 ?? string.Empty);
        AddColumn("Vendor City", x => x.VendorCity ?? string.Empty);
        AddColumn("Vendor State", x => x.VendorState ?? string.Empty);
        AddColumn("Vendor Zip", x => x.VendorZip ?? string.Empty);
        AddColumn("Remarks", x => x.Remarks ?? string.Empty);
        AddColumn("Vendor Phone", x => x.VendorPhone.ToPhoneNumberString() ?? string.Empty);
        AddColumn("Vendor Status", x => x.VendorStatus ?? string.Empty);
        AddColumn("Taxpayer ID Number", x => x.TaxpayerIdNumber ?? string.Empty);
        AddColumn("Email", x => x.Email ?? string.Empty);
    }
}
