using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("AgreementProject")]
public partial class AgreementProject
{
    [Key]
    public int AgreementProjectID { get; set; }

    public int AgreementID { get; set; }

    public int ProjectID { get; set; }

    [ForeignKey("AgreementID")]
    [InverseProperty("AgreementProjects")]
    public virtual Agreement Agreement { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("AgreementProjects")]
    public virtual Project Project { get; set; } = null!;
}
