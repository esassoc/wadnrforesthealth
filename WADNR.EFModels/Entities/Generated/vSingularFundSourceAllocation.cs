using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Keyless]
public partial class vSingularFundSourceAllocation
{
    public int FundSourceID { get; set; }

    public int FundSourceAllocationID { get; set; }
}
