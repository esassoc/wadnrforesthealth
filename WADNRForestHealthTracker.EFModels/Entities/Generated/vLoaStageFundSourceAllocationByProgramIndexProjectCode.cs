using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Keyless]
public partial class vLoaStageFundSourceAllocationByProgramIndexProjectCode
{
    public int LoaStageID { get; set; }

    public int? FundSourceAllocationID { get; set; }

    public int? FundSourceID { get; set; }

    public bool IsNortheast { get; set; }

    public bool? IsSoutheast { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProgramIndex { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProjectCode { get; set; }
}
