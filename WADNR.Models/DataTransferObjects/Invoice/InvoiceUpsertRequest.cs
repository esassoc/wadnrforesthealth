using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects.Invoice;

public class InvoiceUpsertRequest
{
    [Required]
    public int InvoicePaymentRequestID { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? InvoiceIdentifyingName { get; set; }

    [Required]
    public DateTime InvoiceDate { get; set; }

    public decimal? PaymentAmount { get; set; }

    public decimal? MatchAmount { get; set; }

    [Required]
    public int InvoiceMatchAmountTypeID { get; set; }

    public int? FundSourceID { get; set; }

    [MaxLength(255)]
    public string? Fund { get; set; }

    [MaxLength(255)]
    public string? Appn { get; set; }

    [MaxLength(255)]
    public string? SubObject { get; set; }

    public int? ProgramIndexID { get; set; }

    public int? ProjectCodeID { get; set; }

    public int? OrganizationCodeID { get; set; }

    [Required]
    public int InvoiceStatusID { get; set; }

    [Required]
    public int InvoiceApprovalStatusID { get; set; }

    public string? InvoiceApprovalStatusComment { get; set; }
}
