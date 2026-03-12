namespace WADNR.Models.DataTransferObjects.ProjectUpdate;

public class ProjectUpdateStatusGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? FhtProjectNumber { get; set; }
    public string? ProjectStageName { get; set; }
    public string? LeadImplementerOrganizationName { get; set; }
    public decimal? EstimatedTotalCost { get; set; }

    public string? OrganizationPrimaryContactName { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? TotalFunding { get; set; }

    // Latest batch info (null if no batch exists = "Not Started")
    public int? ProjectUpdateBatchID { get; set; }
    public int? ProjectUpdateStateID { get; set; }
    public string? ProjectUpdateStateName { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public string? LastUpdatedByPersonName { get; set; }
    public DateTime? LastSubmittedDate { get; set; }
    public DateTime? LastApprovedDate { get; set; }

    // For client-side filtering (admin sees all projects but needs to identify "my" ones)
    public bool IsMyProject { get; set; }
}
