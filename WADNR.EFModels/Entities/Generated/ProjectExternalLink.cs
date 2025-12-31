using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectExternalLink")]
public partial class ProjectExternalLink
{
    [Key]
    public int ProjectExternalLinkID { get; set; }

    public int ProjectID { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string ExternalLinkLabel { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string ExternalLinkUrl { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectExternalLinks")]
    public virtual Project Project { get; set; } = null!;
}
