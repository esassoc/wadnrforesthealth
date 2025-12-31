using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceNoteInternal")]
public partial class FundSourceNoteInternal
{
    [Key]
    public int FundSourceNoteInternalID { get; set; }

    public int FundSourceID { get; set; }

    [Unicode(false)]
    public string? FundSourceNoteText { get; set; }

    public int CreatedByPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public int? LastUpdatedByPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDate { get; set; }

    [ForeignKey("CreatedByPersonID")]
    [InverseProperty("FundSourceNoteInternalCreatedByPeople")]
    public virtual Person CreatedByPerson { get; set; } = null!;

    [ForeignKey("FundSourceID")]
    [InverseProperty("FundSourceNoteInternals")]
    public virtual FundSource FundSource { get; set; } = null!;

    [ForeignKey("LastUpdatedByPersonID")]
    [InverseProperty("FundSourceNoteInternalLastUpdatedByPeople")]
    public virtual Person? LastUpdatedByPerson { get; set; }
}
