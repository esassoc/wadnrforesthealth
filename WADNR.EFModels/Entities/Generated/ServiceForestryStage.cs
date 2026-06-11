using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ServiceForestryStage")]
[Index("ProjectIdentifier", Name = "IDX_ServiceForestryStageProjectIdentifier")]
public partial class ServiceForestryStage
{
    [Key]
    public int ServiceForestryStageID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? RegionTitle { get; set; }

    [StringLength(600)]
    [Unicode(false)]
    public string ProjectIdentifier { get; set; } = null!;

    public DateOnly? ApprovalDate { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? County { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? Forester { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAcres { get; set; }

    public bool? StewardshipPlan { get; set; }

    [Column(TypeName = "decimal(9, 4)")]
    public decimal? PercentMatch { get; set; }

    [StringLength(600)]
    [Unicode(false)]
    public string? FundSource { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DCStatus { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCAllocatedAmount { get; set; }

    public DateOnly? DCLetterDate { get; set; }

    public DateOnly? DCExpirationDate { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DCTreatment1 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCost1 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCostPerAcre1 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DCAcresTreatment1 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DCTreatment2 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCost2 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCostPerAcre2 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DCAcresTreatment2 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DCTreatment3 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCost3 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCostPerAcre3 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DCAcresTreatment3 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DCTreatment4 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCost4 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCostPerAcre4 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DCAcresTreatment4 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DCTreatment5 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCost5 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCostPerAcre5 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DCAcresTreatment5 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DCTreatment6 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCost6 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCCostPerAcre6 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DCAcresTreatment6 { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCTotalMaxAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DCTreatedAcres { get; set; }

    public bool? DCContractor { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? DCVendorName1 { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? DCVendorName2 { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? DCVendorAddress1 { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? DCVendorAddress2 { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? DCSwvVendorNumber { get; set; }

    public DateOnly? DCInvoiceDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? DCProgramIndex { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? DCProjectCode { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCMatchAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? DCPayAmount { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ItemType { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? SourcePath { get; set; }
}
