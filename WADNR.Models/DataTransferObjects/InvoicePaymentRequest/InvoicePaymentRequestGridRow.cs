namespace WADNR.Models.DataTransferObjects.InvoicePaymentRequest;

public class InvoicePaymentRequestGridRow
{
    public int InvoicePaymentRequestID { get; set; }
    public int ProjectID { get; set; }
    public DateTime InvoicePaymentRequestDate { get; set; }
    public int? VendorID { get; set; }
    public string? VendorName { get; set; }
    public string? VendorAddress { get; set; }
    public string? VendorStatewideVendorNumber { get; set; }
    public int? PreparedByPersonID { get; set; }
    public string? PreparedByPersonFullName { get; set; }
    public string? PreparedByPersonPhone { get; set; }
    public string? PurchaseAuthority { get; set; }
    public bool PurchaseAuthorityIsLandownerCostShareAgreement { get; set; }
    public string? Duns { get; set; }
    public string? Notes { get; set; }
    public int InvoiceCount { get; set; }
}
