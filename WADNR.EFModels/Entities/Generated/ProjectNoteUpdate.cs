using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectNoteUpdate")]
public partial class ProjectNoteUpdate
{
    [Key]
    public int ProjectNoteUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

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
    [InverseProperty("ProjectNoteUpdateCreatePeople")]
    public virtual Person? CreatePerson { get; set; }

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectNoteUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;

    [ForeignKey("UpdatePersonID")]
    [InverseProperty("ProjectNoteUpdateUpdatePeople")]
    public virtual Person? UpdatePerson { get; set; }
}
