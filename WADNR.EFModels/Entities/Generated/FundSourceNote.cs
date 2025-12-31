using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceNote")]
public partial class FundSourceNote
{
    [Key]
    public int FundSourceNoteID { get; set; }

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
    [InverseProperty("FundSourceNoteCreatedByPeople")]
    public virtual Person CreatedByPerson { get; set; } = null!;

    [ForeignKey("FundSourceID")]
    [InverseProperty("FundSourceNotes")]
    public virtual FundSource FundSource { get; set; } = null!;

    [ForeignKey("LastUpdatedByPersonID")]
    [InverseProperty("FundSourceNoteLastUpdatedByPeople")]
    public virtual Person? LastUpdatedByPerson { get; set; }
}
