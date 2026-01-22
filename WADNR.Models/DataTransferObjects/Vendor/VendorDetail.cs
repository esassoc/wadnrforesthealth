namespace WADNR.Models.DataTransferObjects;

public class VendorDetail
{
    public int VendorID { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string StatewideVendorNumber { get; set; } = string.Empty;
    public string StatewideVendorNumberSuffix { get; set; } = string.Empty;
    public string? VendorType { get; set; }
    public string? VendorStatus { get; set; }

    // Billing Info
    public string? BillingAgency { get; set; }
    public string? BillingSubAgency { get; set; }
    public string? BillingFund { get; set; }
    public string? BillingFundBreakout { get; set; }

    // Address
    public string? VendorAddressLine1 { get; set; }
    public string? VendorAddressLine2 { get; set; }
    public string? VendorAddressLine3 { get; set; }
    public string? VendorCity { get; set; }
    public string? VendorState { get; set; }
    public string? VendorZip { get; set; }

    // Contact
    public string? VendorPhone { get; set; }
    public string? Email { get; set; }

    // Other
    public string? Remarks { get; set; }
    public string? TaxpayerIdNumber { get; set; }

    // Counts
    public int PersonCount { get; set; }
    public int OrganizationCount { get; set; }
}
