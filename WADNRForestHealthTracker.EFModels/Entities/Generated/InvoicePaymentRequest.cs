using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("InvoicePaymentRequest")]
public partial class InvoicePaymentRequest
{
    [Key]
    public int InvoicePaymentRequestID { get; set; }

    public int ProjectID { get; set; }

    public int? VendorID { get; set; }

    public int? PreparedByPersonID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? PurchaseAuthority { get; set; }

    public bool PurchaseAuthorityIsLandownerCostShareAgreement { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Duns { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime InvoicePaymentRequestDate { get; set; }

    [Unicode(false)]
    public string? Notes { get; set; }

    [InverseProperty("InvoicePaymentRequest")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("PreparedByPersonID")]
    [InverseProperty("InvoicePaymentRequests")]
    public virtual Person? PreparedByPerson { get; set; }

    [ForeignKey("ProjectID")]
    [InverseProperty("InvoicePaymentRequests")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("VendorID")]
    [InverseProperty("InvoicePaymentRequests")]
    public virtual Vendor? Vendor { get; set; }
}
