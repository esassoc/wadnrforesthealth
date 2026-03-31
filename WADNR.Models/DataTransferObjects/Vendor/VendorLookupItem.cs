namespace WADNR.Models.DataTransferObjects;

public class VendorLookupItem
{
    public int VendorID { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string StatewideVendorNumber { get; set; } = string.Empty;
    public string StatewideVendorNumberSuffix { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
