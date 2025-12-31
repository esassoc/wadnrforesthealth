using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    public int InvoiceID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? InvoiceIdentifyingName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime InvoiceDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? PaymentAmount { get; set; }

    public int InvoiceApprovalStatusID { get; set; }

    [Unicode(false)]
    public string? InvoiceApprovalStatusComment { get; set; }

    public int InvoiceMatchAmountTypeID { get; set; }

    [Column(TypeName = "money")]
    public decimal? MatchAmount { get; set; }

    public int InvoiceStatusID { get; set; }

    public int? InvoiceFileResourceID { get; set; }

    public int InvoicePaymentRequestID { get; set; }

    public int? FundSourceID { get; set; }

    public int? ProgramIndexID { get; set; }

    public int? ProjectCodeID { get; set; }

    public int? OrganizationCodeID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string InvoiceNumber { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Fund { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Appn { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? SubObject { get; set; }

    [ForeignKey("FundSourceID")]
    [InverseProperty("Invoices")]
    public virtual FundSource? FundSource { get; set; }

    [ForeignKey("InvoiceApprovalStatusID")]
    [InverseProperty("Invoices")]
    public virtual InvoiceApprovalStatus InvoiceApprovalStatus { get; set; } = null!;

    [ForeignKey("InvoiceFileResourceID")]
    [InverseProperty("Invoices")]
    public virtual FileResource? InvoiceFileResource { get; set; }

    [ForeignKey("InvoicePaymentRequestID")]
    [InverseProperty("Invoices")]
    public virtual InvoicePaymentRequest InvoicePaymentRequest { get; set; } = null!;

    [ForeignKey("ProgramIndexID")]
    [InverseProperty("Invoices")]
    public virtual ProgramIndex? ProgramIndex { get; set; }

    [ForeignKey("ProjectCodeID")]
    [InverseProperty("Invoices")]
    public virtual ProjectCode? ProjectCode { get; set; }
}
