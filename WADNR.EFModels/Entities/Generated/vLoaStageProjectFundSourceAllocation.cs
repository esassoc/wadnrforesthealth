using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Keyless]
public partial class vLoaStageProjectFundSourceAllocation
{
    public int ProjectID { get; set; }

    [StringLength(140)]
    [Unicode(false)]
    public string? ProjectGisIdentifier { get; set; }

    [Column(TypeName = "money")]
    public decimal? MatchAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? PayAmount { get; set; }

    [StringLength(600)]
    [Unicode(false)]
    public string? ProjectStatus { get; set; }

    public int? FundSourceAllocationID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LetterDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ProjectExpirationDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ApplicationDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DecisionDate { get; set; }

    public int LoaStageID { get; set; }

    public bool IsNortheast { get; set; }

    public bool? IsSoutheast { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProgramIndex { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProjectCode { get; set; }
}
