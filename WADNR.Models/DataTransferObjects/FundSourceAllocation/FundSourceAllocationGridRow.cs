namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationGridRow
{
    public int FundSourceAllocationID { get; set; }
    public string? FundSourceAllocationName { get; set; }
    public int FundSourceID { get; set; }
    public string FundSourceNumber { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? AllocationAmount { get; set; }
    public string? DNRUplandRegionName { get; set; }
    public int? DNRUplandRegionID { get; set; }
    public string? OrganizationName { get; set; }
    public int? OrganizationID { get; set; }
    public string? FundSourceAllocationPriorityName { get; set; }
    public string? FundSourceAllocationPriorityColor { get; set; }
    public bool? HasFundFSPs { get; set; }
    public int ProjectCount { get; set; }
    public string? FundSourceManagerName { get; set; }
    public string? ProgramManagerNames { get; set; }
    public int? FundSourceStatusID { get; set; }
    public string? FundSourceStatusName { get; set; }
    public int? DivisionID { get; set; }
    public string? DivisionName { get; set; }
    public string? FederalFundCodeAbbrev { get; set; }
    public string? ProgramIndexProjectCodeDisplay { get; set; }
}
