namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationExcelRow
{
    public string FundSourceNumber { get; set; } = string.Empty;
    public string? FundSourceAllocationName { get; set; }
    public string? ProgramManagerNames { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? ParentFundSourceStatusName { get; set; }
    public string? DNRUplandRegionName { get; set; }
    public string? FederalFundCodeDisplay { get; set; }
    public decimal? AllocationAmount { get; set; }
    public string? ProgramIndexProjectCodeDisplay { get; set; }
    public string? OrganizationName { get; set; }
}
