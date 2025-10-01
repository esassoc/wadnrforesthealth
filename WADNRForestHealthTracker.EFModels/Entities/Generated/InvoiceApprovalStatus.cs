using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("InvoiceApprovalStatus")]
[Index("InvoiceApprovalStatusName", Name = "AK_InvoiceApprovalStatus_InvoiceApprovalStatusName", IsUnique = true)]
public partial class InvoiceApprovalStatus
{
    [Key]
    public int InvoiceApprovalStatusID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string InvoiceApprovalStatusName { get; set; } = null!;

    [InverseProperty("InvoiceApprovalStatus")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
