using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("Vendor")]
[Index("StatewideVendorNumber", "StatewideVendorNumberSuffix", Name = "AK_Vendor_StatewideVendorNumber_StatewideVendorNumberSuffix", IsUnique = true)]
public partial class Vendor
{
    [Key]
    public int VendorID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string VendorName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string StatewideVendorNumber { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string StatewideVendorNumberSuffix { get; set; } = null!;

    [StringLength(3)]
    [Unicode(false)]
    public string? VendorType { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? BillingAgency { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? BillingSubAgency { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? BillingFund { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? BillingFundBreakout { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VendorAddressLine1 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VendorAddressLine2 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VendorAddressLine3 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VendorCity { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VendorState { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VendorZip { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Remarks { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VendorPhone { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VendorStatus { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? TaxpayerIdNumber { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Email { get; set; }

    [InverseProperty("Vendor")]
    public virtual ICollection<InvoicePaymentRequest> InvoicePaymentRequests { get; set; } = new List<InvoicePaymentRequest>();

    [InverseProperty("Vendor")]
    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    [InverseProperty("Vendor")]
    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
