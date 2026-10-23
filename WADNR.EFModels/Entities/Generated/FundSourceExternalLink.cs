using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceExternalLink")]
[Index("FundSourceID", Name = "IX_FundSourceExternalLink_FundSourceID")]
public partial class FundSourceExternalLink
{
    [Key]
    public int FundSourceExternalLinkID { get; set; }

    public int FundSourceID { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string ExternalLinkLabel { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string ExternalLinkUrl { get; set; } = null!;

    [ForeignKey("FundSourceID")]
    [InverseProperty("FundSourceExternalLinks")]
    public virtual FundSource FundSource { get; set; } = null!;
}
