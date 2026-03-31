using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectExternalLinkUpdate")]
public partial class ProjectExternalLinkUpdate
{
    [Key]
    public int ProjectExternalLinkUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string ExternalLinkLabel { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string ExternalLinkUrl { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectExternalLinkUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
