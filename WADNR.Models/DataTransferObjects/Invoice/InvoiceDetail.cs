namespace WADNR.Models.DataTransferObjects.Invoice;

public class InvoiceDetail
{
    public int InvoiceID { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? InvoiceIdentifyingName { get; set; }
    public DateOnly InvoiceDate { get; set; }

    // Payment Request info
    public int InvoicePaymentRequestID { get; set; }
    public DateOnly InvoicePaymentRequestDate { get; set; }
    public string? PurchaseAuthority { get; set; }
    public bool PurchaseAuthorityIsLandownerCostShareAgreement { get; set; }
    public string? Duns { get; set; }
    public string? PaymentRequestNotes { get; set; }

    // Project info
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;

    // Vendor info
    public int? VendorID { get; set; }
    public string? VendorName { get; set; }

    // Prepared By info
    public int? PreparedByPersonID { get; set; }
    public string? PreparedByPersonName { get; set; }

    // Fund Source info
    public int? FundSourceID { get; set; }
    public string? FundSourceNumber { get; set; }
    public string? FundSourceName { get; set; }

    // Financial codes
    public string? Fund { get; set; }
    public string? Appn { get; set; }
    public string? SubObject { get; set; }

    // Program Index info
    public int? ProgramIndexID { get; set; }
    public string? ProgramIndexCode { get; set; }
    public string? ProgramIndexTitle { get; set; }

    // Project Code info
    public int? ProjectCodeID { get; set; }
    public string? ProjectCodeName { get; set; }
    public string? ProjectCodeTitle { get; set; }

    // Organization Code info
    public int? OrganizationCodeID { get; set; }
    public string? OrganizationCodeName { get; set; }
    public string? OrganizationCodeValue { get; set; }

    // Amounts
    public decimal? PaymentAmount { get; set; }
    public decimal? MatchAmount { get; set; }

    // Match Amount Type
    public int InvoiceMatchAmountTypeID { get; set; }
    public string InvoiceMatchAmountTypeDisplayName { get; set; } = string.Empty;

    // Status
    public int InvoiceStatusID { get; set; }
    public string InvoiceStatusDisplayName { get; set; } = string.Empty;

    // Approval Status
    public int InvoiceApprovalStatusID { get; set; }
    public string InvoiceApprovalStatusName { get; set; } = string.Empty;
    public string? InvoiceApprovalStatusComment { get; set; }

    // Invoice Voucher File
    public int? InvoiceFileResourceID { get; set; }
    public Guid? InvoiceFileResourceGuid { get; set; }
    public string? InvoiceFileOriginalFileName { get; set; }
}
