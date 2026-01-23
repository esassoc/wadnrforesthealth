namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationDetail
{
    public int FundSourceAllocationID { get; set; }
    public string? FundSourceAllocationName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? AllocationAmount { get; set; }
    public bool? HasFundFSPs { get; set; }
    public bool? LikelyToUse { get; set; }

    // Fund Source
    public int FundSourceID { get; set; }
    public string FundSourceNumber { get; set; } = string.Empty;

    // DNR Upland Region
    public int? DNRUplandRegionID { get; set; }
    public string? DNRUplandRegionName { get; set; }

    // Organization
    public int? OrganizationID { get; set; }
    public string? OrganizationName { get; set; }

    // Federal Fund Code
    public int? FederalFundCodeID { get; set; }
    public string? FederalFundCodeName { get; set; }

    // Division
    public int? DivisionID { get; set; }
    public string? DivisionName { get; set; }

    // Fund Source Manager
    public int? FundSourceManagerID { get; set; }
    public string? FundSourceManagerName { get; set; }

    // Priority
    public int? FundSourceAllocationPriorityID { get; set; }
    public string? FundSourceAllocationPriorityName { get; set; }
    public string? FundSourceAllocationPriorityColor { get; set; }

    // Source
    public int? FundSourceAllocationSourceID { get; set; }
    public string? FundSourceAllocationSourceName { get; set; }

    // Related counts
    public int ProjectCount { get; set; }
    public int AgreementCount { get; set; }
    public int ProgramIndexCount { get; set; }
    public int ProjectCodeCount { get; set; }
}
