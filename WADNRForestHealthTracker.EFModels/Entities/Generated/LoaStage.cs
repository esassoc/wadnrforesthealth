using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("LoaStage")]
[Index("FundSourceNumber", Name = "IDX_LoaStageGrantNumber")]
public partial class LoaStage
{
    [Key]
    public int LoaStageID { get; set; }

    [StringLength(600)]
    [Unicode(false)]
    public string ProjectIdentifier { get; set; } = null!;

    [StringLength(600)]
    [Unicode(false)]
    public string? ProjectStatus { get; set; }

    [StringLength(600)]
    [Unicode(false)]
    public string? FundSourceNumber { get; set; }

    [StringLength(600)]
    [Unicode(false)]
    public string? FocusAreaName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ProjectExpirationDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LetterDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? MatchAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? PayAmount { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProgramIndex { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProjectCode { get; set; }

    public bool IsNortheast { get; set; }

    public bool IsSoutheast { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ForesterLastName { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ForesterFirstName { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ForesterPhone { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ForesterEmail { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ApplicationDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DecisionDate { get; set; }
}
