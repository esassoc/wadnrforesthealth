using System;

namespace WADNR.Models.DataTransferObjects.Invoice;

public class InvoiceApiJson
{
    public int InvoiceID { get; set; }
    public string InvoiceIdentifyingName { get; set; }
    public string RequestorName { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public string PurchaseAuthority { get; set; }
    public decimal? PaymentAmount { get; set; }
    public int? PreparedByPersonID { get; set; }
    public string PreparedByPersonName { get; set; }
    public int InvoiceApprovalStatusID { get; set; }
    public string InvoiceApprovalStatusName { get; set; }
    public string InvoiceApprovalStatusComment { get; set; }
    public bool PurchaseAuthorityIsLandownerCostShareAgreement { get; set; }
    public int InvoiceMatchAmountTypeID { get; set; }
    public decimal? MatchAmount { get; set; }
    public int InvoiceStatusID { get; set; }
    public string InvoiceStatusName { get; set; }
}
