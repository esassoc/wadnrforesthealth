namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Basics step of the Project Update workflow.
/// </summary>
public class ProjectUpdateBasicsStep
{
    public int ProjectUpdateBatchID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public int ProjectTypeID { get; set; }
    public string ProjectTypeName { get; set; } = string.Empty;
    public int ProjectStageID { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public int? FocusAreaID { get; set; }
    public int? LeadImplementerOrganizationID { get; set; }
    public int? PercentageMatch { get; set; }
    public List<int> ProgramIDs { get; set; } = new();

    // GIS import flags — when true, the field is read-only (managed by external GIS system)
    public bool IsProjectStageImported { get; set; }
    public bool IsPlannedDateImported { get; set; }
    public bool IsCompletionDateImported { get; set; }
}

/// <summary>
/// Request for saving the Basics step of the Project Update workflow.
/// </summary>
public class ProjectUpdateBasicsStepRequest
{
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public int ProjectTypeID { get; set; }
    public int ProjectStageID { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public int? FocusAreaID { get; set; }
    public int? LeadImplementerOrganizationID { get; set; }
    public int? PercentageMatch { get; set; }
    public List<int> ProgramIDs { get; set; } = new();
}
