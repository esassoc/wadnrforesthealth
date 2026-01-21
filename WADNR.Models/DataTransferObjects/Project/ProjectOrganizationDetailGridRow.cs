namespace WADNR.Models.DataTransferObjects;

public class ProjectOrganizationDetailGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string FhtProjectNumber { get; set; } = string.Empty;
    public OrganizationLookupItem? PrimaryContactOrganization { get; set; }
    public OrganizationLookupItem? ProjectStewardOrganization { get; set; }
    public ProjectStageLookupItem ProjectStage { get; set; } = null!;
    public string RelationshipTypes { get; set; } = string.Empty;
    public DateTime? ProjectInitiationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? ProjectDescription { get; set; }
    public int PhotoCount { get; set; }
}
