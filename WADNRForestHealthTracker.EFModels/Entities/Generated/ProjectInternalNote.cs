using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectInternalNote")]
public partial class ProjectInternalNote
{
    [Key]
    public int ProjectInternalNoteID { get; set; }

    public int ProjectID { get; set; }

    [StringLength(8000)]
    [Unicode(false)]
    public string Note { get; set; } = null!;

    public int? CreatePersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int? UpdatePersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [ForeignKey("CreatePersonID")]
    [InverseProperty("ProjectInternalNoteCreatePeople")]
    public virtual Person? CreatePerson { get; set; }

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectInternalNotes")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("UpdatePersonID")]
    [InverseProperty("ProjectInternalNoteUpdatePeople")]
    public virtual Person? UpdatePerson { get; set; }
}
