namespace WADNR.Models.DataTransferObjects.Invoice;

public class InvoiceGridRow
{
    public int InvoiceID { get; set; }
    public int InvoicePaymentRequestID { get; set; }
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int? FundSourceID { get; set; }
    public string? FundSourceNumber { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public string? Fund { get; set; }
    public string? Appn { get; set; }
    public string? ProgramIndexCode { get; set; }
    public string? ProjectCodeName { get; set; }
    public string? SubObject { get; set; }
    public int? OrganizationCodeID { get; set; }
    public string? OrganizationCodeName { get; set; }
    public decimal? MatchAmount { get; set; }
    public decimal? PaymentAmount { get; set; }
    public int InvoiceStatusID { get; set; }
    public string InvoiceStatusDisplayName { get; set; } = string.Empty;
    public int InvoiceApprovalStatusID { get; set; }
    public string InvoiceApprovalStatusName { get; set; } = string.Empty;
    public string? InvoiceIdentifyingName { get; set; }
    public Guid? InvoiceFileResourceGuid { get; set; }
}
