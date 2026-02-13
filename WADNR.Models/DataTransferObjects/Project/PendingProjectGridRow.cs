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
    public DateTime? ProjectInitiationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public string? ProjectDescription { get; set; }
}
