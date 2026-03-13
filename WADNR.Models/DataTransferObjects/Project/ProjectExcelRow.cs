namespace WADNR.Models.DataTransferObjects;

public class ProjectExcelRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string FhtProjectNumber { get; set; } = string.Empty;
    public string? ProjectGisIdentifier { get; set; }
    public string ProgramNames { get; set; } = string.Empty;
    public string NonLeadImplementingOrganizations { get; set; } = string.Empty;
    public string ProjectStageName { get; set; } = string.Empty;
    public string ProjectThemes { get; set; } = string.Empty;
    public string PriorityLandscapeNames { get; set; } = string.Empty;
    public string DNRUplandRegionNames { get; set; } = string.Empty;
    public string CountyNames { get; set; } = string.Empty;
    public string? FocusAreaName { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? ProjectDescription { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? TotalFundingAmount { get; set; }
    public string? ProjectLocationNotes { get; set; }
}
