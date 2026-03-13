using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects.InvoicePaymentRequest;

public class InvoicePaymentRequestUpsertRequest
{
    [Required]
    public int ProjectID { get; set; }

    public int? VendorID { get; set; }

    [Required]
    public int? PreparedByPersonID { get; set; }

    [Required]
    public DateOnly InvoicePaymentRequestDate { get; set; }

    [MaxLength(255)]
    public string? PurchaseAuthority { get; set; }

    public bool PurchaseAuthorityIsLandownerCostShareAgreement { get; set; }

    [MaxLength(20)]
    public string? Duns { get; set; }

    public string? Notes { get; set; }
}
