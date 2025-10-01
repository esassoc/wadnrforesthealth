using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FundSourceAllocationLikelyPerson")]
[Index("FundSourceAllocationID", "PersonID", Name = "AK_FundSourceAllocationLikelyPerson_FundSourceAllocationID_PersonID", IsUnique = true)]
public partial class FundSourceAllocationLikelyPerson
{
    [Key]
    public int FundSourceAllocationLikelyPersonID { get; set; }

    public int FundSourceAllocationID { get; set; }

    public int PersonID { get; set; }

    [ForeignKey("FundSourceAllocationID")]
    [InverseProperty("FundSourceAllocationLikelyPeople")]
    public virtual FundSourceAllocation FundSourceAllocation { get; set; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("FundSourceAllocationLikelyPeople")]
    public virtual Person Person { get; set; } = null!;
}
