using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FundSourceAllocationNote")]
public partial class FundSourceAllocationNote
{
    [Key]
    public int FundSourceAllocationNoteID { get; set; }

    public int FundSourceAllocationID { get; set; }

    [Unicode(false)]
    public string? FundSourceAllocationNoteText { get; set; }

    public int CreatedByPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public int? LastUpdatedByPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDate { get; set; }

    [ForeignKey("CreatedByPersonID")]
    [InverseProperty("FundSourceAllocationNoteCreatedByPeople")]
    public virtual Person CreatedByPerson { get; set; } = null!;

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationNotes")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;

    [ForeignKey("LastUpdatedByPersonID")]
    [InverseProperty("FundSourceAllocationNoteLastUpdatedByPeople")]
    public virtual Person? LastUpdatedByPerson { get; set; }
}
