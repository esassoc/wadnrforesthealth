using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSource")]
public partial class FundSource
{
    [Key]
    public int FundSourceID { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? FundSourceNumber { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Unicode(false)]
    public string? ConditionsAndRequirements { get; set; }

    [Unicode(false)]
    public string? ComplianceNotes { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? CFDANumber { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string FundSourceName { get; set; } = null!;

    public int? FundSourceTypeID { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string? ShortName { get; set; }

    public int FundSourceStatusID { get; set; }

    public int OrganizationID { get; set; }

    [Column(TypeName = "money")]
    public decimal TotalAwardAmount { get; set; }

    [InverseProperty("FundSource")]
    public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; } = new List<FundSourceAllocation>();

    [InverseProperty("FundSource")]
    public virtual ICollection<FundSourceFileResource> FundSourceFileResources { get; set; } = new List<FundSourceFileResource>();

    [InverseProperty("FundSource")]
    public virtual ICollection<FundSourceNoteInternal> FundSourceNoteInternals { get; set; } = new List<FundSourceNoteInternal>();

    [InverseProperty("FundSource")]
    public virtual ICollection<FundSourceNote> FundSourceNotes { get; set; } = new List<FundSourceNote>();

    [ForeignKey("FundSourceTypeID")]
    [InverseProperty("FundSources")]
    public virtual FundSourceType? FundSourceType { get; set; }

    [InverseProperty("FundSource")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("OrganizationID")]
    [InverseProperty("FundSources")]
    public virtual Organization Organization { get; set; } = null!;
}
