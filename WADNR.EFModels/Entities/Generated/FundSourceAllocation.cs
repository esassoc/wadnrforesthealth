using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FundSourceAllocation")]
public partial class FundSourceAllocation
{
    [Key]
    public int FundSourceAllocationID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? FundSourceAllocationName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? AllocationAmount { get; set; }

    public int? FederalFundCodeID { get; set; }

    public int? OrganizationID { get; set; }

    public int? DNRUplandRegionID { get; set; }

    public int? DivisionID { get; set; }

    public int? FundSourceManagerID { get; set; }

    public int? FundSourceAllocationPriorityID { get; set; }

    public bool? HasFundFSPs { get; set; }

    public int? FundSourceAllocationSourceID { get; set; }

    public bool? LikelyToUse { get; set; }

    public int FundSourceID { get; set; }

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<AgreementFundSourceAllocation> AgreementFundSourceAllocations { get; set; } = new List<AgreementFundSourceAllocation>();

    [ForeignKey("DNRUplandRegionID")]
    [InverseProperty("FundSourceAllocations")]
    public virtual DNRUplandRegion? DNRUplandRegion { get; set; }

    [ForeignKey("FederalFundCodeID")]
    [InverseProperty("FundSourceAllocations")]
    public virtual FederalFundCode? FederalFundCode { get; set; }

    [ForeignKey("FundSourceID")]
    [InverseProperty("FundSourceAllocations")]
    public virtual FundSource FundSource { get; set; } = null!;

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationBudgetLineItem> FundSourceAllocationBudgetLineItems { get; set; } = new List<FundSourceAllocationBudgetLineItem>();

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationChangeLog> FundSourceAllocationChangeLogs { get; set; } = new List<FundSourceAllocationChangeLog>();

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationExpenditure> FundSourceAllocationExpenditures { get; set; } = new List<FundSourceAllocationExpenditure>();

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationFileResource> FundSourceAllocationFileResources { get; set; } = new List<FundSourceAllocationFileResource>();

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationLikelyPerson> FundSourceAllocationLikelyPeople { get; set; } = new List<FundSourceAllocationLikelyPerson>();

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationNoteInternal> FundSourceAllocationNoteInternals { get; set; } = new List<FundSourceAllocationNoteInternal>();

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationNote> FundSourceAllocationNotes { get; set; } = new List<FundSourceAllocationNote>();

    [ForeignKey("FundSourceAllocationPriorityID")]
    [InverseProperty("FundSourceAllocations")]
    public virtual FundSourceAllocationPriority? FundSourceAllocationPriority { get; set; }

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationProgramIndexProjectCode> FundSourceAllocationProgramIndexProjectCodes { get; set; } = new List<FundSourceAllocationProgramIndexProjectCode>();

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<FundSourceAllocationProgramManager> FundSourceAllocationProgramManagers { get; set; } = new List<FundSourceAllocationProgramManager>();

    [ForeignKey("FundSourceManagerID")]
    [InverseProperty("FundSourceAllocations")]
    public virtual Person? FundSourceManager { get; set; }

    [ForeignKey("OrganizationID")]
    [InverseProperty("FundSourceAllocations")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<ProjectFundSourceAllocationRequestUpdate> ProjectFundSourceAllocationRequestUpdates { get; set; } = new List<ProjectFundSourceAllocationRequestUpdate>();

    [InverseProperty("FundSourceAllocation")]
    public virtual ICollection<ProjectFundSourceAllocationRequest> ProjectFundSourceAllocationRequests { get; set; } = new List<ProjectFundSourceAllocationRequest>();
}
