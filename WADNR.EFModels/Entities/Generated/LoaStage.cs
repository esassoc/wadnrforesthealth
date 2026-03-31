using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

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

    public DateOnly? ProjectExpirationDate { get; set; }

    public DateOnly? LetterDate { get; set; }

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

    public DateOnly? ApplicationDate { get; set; }

    public DateOnly? DecisionDate { get; set; }
}
