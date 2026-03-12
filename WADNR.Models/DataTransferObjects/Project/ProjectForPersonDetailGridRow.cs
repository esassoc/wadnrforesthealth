namespace WADNR.Models.DataTransferObjects;

public class ProjectForPersonDetailGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string FhtProjectNumber { get; set; } = string.Empty;
    public ProjectTypeLookupItem ProjectType { get; set; } = null!;
    public ProjectStageLookupItem ProjectStage { get; set; } = null!;
    public OrganizationLookupItem? LeadImplementerOrganization { get; set; }
    public string? PrimaryContactOrganization { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? TotalFunding { get; set; }
    public string? ProjectDescription { get; set; }
}
