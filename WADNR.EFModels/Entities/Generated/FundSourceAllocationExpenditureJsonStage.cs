using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceAllocationExpenditureJsonStage")]
public partial class FundSourceAllocationExpenditureJsonStage
{
    [Key]
    public int FundSourceAllocationExpenditureJsonStageID { get; set; }

    public int? Biennium { get; set; }

    public int? FiscalMo { get; set; }

    public int? FiscalAdjMo { get; set; }

    public int? CalYr { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? MoString { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? SourceSystem { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DocNo { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? DocSuffix { get; set; }

    public DateOnly? DocDate { get; set; }

    [Unicode(false)]
    public string? InvoiceDesc { get; set; }

    public DateOnly? InvoiceDate { get; set; }

    public int? GlAcctNo { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ObjCd { get; set; }

    [Unicode(false)]
    public string? ObjName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? SubObjCd { get; set; }

    [Unicode(false)]
    public string? SubObjName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? SubSubObjCd { get; set; }

    [Unicode(false)]
    public string? SubSubObjName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ApprnCd { get; set; }

    [Unicode(false)]
    public string? ApprnName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? FundCd { get; set; }

    [Unicode(false)]
    public string? FundName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? OrgCd { get; set; }

    [Unicode(false)]
    public string? OrgName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ProgIdxCd { get; set; }

    [Unicode(false)]
    public string? ProgIdxName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ProgCd { get; set; }

    [Unicode(false)]
    public string? ProgName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? SubProgCd { get; set; }

    [Unicode(false)]
    public string? SubProgName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ActivityCd { get; set; }

    [Unicode(false)]
    public string? ActivityName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? SubActivityCd { get; set; }

    [Unicode(false)]
    public string? SubActivityName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ProjectCd { get; set; }

    [Unicode(false)]
    public string? ProjectName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? VendorNo { get; set; }

    [Unicode(false)]
    public string? VendorName { get; set; }

    [Column(TypeName = "money")]
    public decimal? ExpendAccrued { get; set; }
}
