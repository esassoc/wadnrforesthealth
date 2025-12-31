using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("AgreementFundSourceAllocation")]
[Index("AgreementID", "FundSourceAllocationID", Name = "AK_AgreementFundSourceAllocation_AgreementID_FundSourceAllocationID", IsUnique = true)]
public partial class AgreementFundSourceAllocation
{
    [Key]
    public int AgreementFundSourceAllocationID { get; set; }

    public int AgreementID { get; set; }

    public int FundSourceAllocationID { get; set; }

    [ForeignKey("AgreementID")]
    [InverseProperty("AgreementFundSourceAllocations")]
    public virtual Agreement Agreement { get; set; } = null!;

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("AgreementFundSourceAllocations")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;
}
