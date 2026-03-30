using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectFundingSource")]
public partial class ProjectFundingSource
{
    [Key]
    public int ProjectFundingSourceID { get; set; }

    public int ProjectID { get; set; }

    public int FundingSourceID { get; set; }

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectFundingSources")]
    public virtual Project Project { get; set; } = null!;
}
