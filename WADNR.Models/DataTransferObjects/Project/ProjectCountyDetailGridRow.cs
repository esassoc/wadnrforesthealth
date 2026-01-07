namespace WADNR.Models.DataTransferObjects;

public class ProjectCountyDetailGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string FhtProjectNumber { get; set; } = string.Empty;
    public OrganizationLookupItem? PrimaryContactOrganization { get; set; }
    public ProjectStageLookupItem ProjectStage { get; set; } = null!;
    public DateTime? ProjectInitiationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? ProjectDescription { get; set; }
}
