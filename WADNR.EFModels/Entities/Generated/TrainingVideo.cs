using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("TrainingVideo")]
public partial class TrainingVideo
{
    [Key]
    public int TrainingVideoID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string VideoName { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? VideoDescription { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime VideoUploadDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string VideoURL { get; set; } = null!;
}
