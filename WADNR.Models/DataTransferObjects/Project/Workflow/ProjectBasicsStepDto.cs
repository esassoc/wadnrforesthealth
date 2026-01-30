namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// DTO for the Basics step of the ProjectCreate wizard.
/// </summary>
public class ProjectBasicsStepDto
{
    public int? ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public int ProjectTypeID { get; set; }
    public int ProjectStageID { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int? FocusAreaID { get; set; }
    public int? LeadImplementerOrganizationID { get; set; }
    public int? PercentageMatch { get; set; }
    public List<int> ProgramIDs { get; set; } = new();
}

/// <summary>
/// Request DTO for saving the Basics step.
/// </summary>
public class ProjectBasicsStepRequestDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public int ProjectTypeID { get; set; }
    public int ProjectStageID { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int? FocusAreaID { get; set; }
    public int? LeadImplementerOrganizationID { get; set; }
    public int? PercentageMatch { get; set; }
    public List<int> ProgramIDs { get; set; } = new();
}
