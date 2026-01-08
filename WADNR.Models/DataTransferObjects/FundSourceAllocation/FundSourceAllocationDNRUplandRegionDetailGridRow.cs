namespace WADNR.Models.DataTransferObjects;

using System;
using System.Collections.Generic;

public class FundSourceAllocationDNRUplandRegionDetailGridRow
{
    public int FundSourceAllocationID { get; set; }
    public string? FundSourceAllocationName { get; set; }
    public DateTime? FundSourceEndDate { get; set; }
    public bool? HasFundFSPs { get; set; }
    public FundSourceAllocationPriorityDetail FundSourceAllocationPriorityDetail { get; set; } = new FundSourceAllocationPriorityDetail();
    public List<ProgramIndexLookupItem> ProgramIndexLookupItems { get; set; } = new List<ProgramIndexLookupItem>();
    public List<ProjectCodeLookupItem> ProjectCodeLookupItems { get; set; } = new List<ProjectCodeLookupItem>();
    public FundSourceLookupItem FundSource { get; set; } = new FundSourceLookupItem();
    public FundSourceAllocationSourceLookupItem? FundSourceAllocationSource { get; set; }
    public decimal? ExpectedFundingByProject { get; set; }
    public decimal? AllocationAmount { get; set; }
    public decimal? BudgetLineItem { get; set; }
    public List<PersonLookupItem> LikelyToUsePeople { get; set; } = new List<PersonLookupItem>();
}
