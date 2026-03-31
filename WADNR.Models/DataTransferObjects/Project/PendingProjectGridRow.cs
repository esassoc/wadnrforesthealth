namespace WADNR.Models.DataTransferObjects;

public class PendingProjectGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string FhtProjectNumber { get; set; } = string.Empty;
    public ProjectTypeLookupItem ProjectType { get; set; } = null!;
    public ProjectStageLookupItem ProjectStage { get; set; } = null!;
    public int ProjectApprovalStatusID { get; set; }
    public string ProjectApprovalStatusName { get; set; } = string.Empty;
    public OrganizationLookupItem? LeadImplementerOrganization { get; set; }
    public List<ProgramLookupItem> Programs { get; set; } = new List<ProgramLookupItem>();
    public PriorityLandscapeLookupItem? PriorityLandscape { get; set; }
    public CountyLookupItem? County { get; set; }
    public DateOnly? ProjectInitiationDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTimeOffset? SubmittedDate { get; set; }
    public DateTimeOffset? LastUpdatedDate { get; set; }
    public string? ProjectDescription { get; set; }
}
