using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceAllocationNoteInternal")]
public partial class FundSourceAllocationNoteInternal
{
    [Key]
    public int FundSourceAllocationNoteInternalID { get; set; }

    public int FundSourceAllocationID { get; set; }

    [Unicode(false)]
    public string? FundSourceAllocationNoteInternalText { get; set; }

    public int CreatedByPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public int? LastUpdatedByPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDate { get; set; }

    [ForeignKey("CreatedByPersonID")]
    [InverseProperty("FundSourceAllocationNoteInternalCreatedByPeople")]
    public virtual Person CreatedByPerson { get; set; } = null!;

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationNoteInternals")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;

    [ForeignKey("LastUpdatedByPersonID")]
    [InverseProperty("FundSourceAllocationNoteInternalLastUpdatedByPeople")]
    public virtual Person? LastUpdatedByPerson { get; set; }
}
