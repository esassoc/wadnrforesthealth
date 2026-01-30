namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// DTO for the Classifications step of the ProjectCreate wizard.
/// Note: Available classification systems should be fetched from GET /lookups/classification-systems-with-classifications
/// </summary>
public class ProjectClassificationsStepDto
{
    public int ProjectID { get; set; }
    /// <summary>
    /// Currently selected classifications for the project.
    /// </summary>
    public List<ProjectClassificationStepItem> Classifications { get; set; } = new();
}

/// <summary>
/// A classification assigned to a project.
/// </summary>
public class ProjectClassificationStepItem
{
    public int? ProjectClassificationID { get; set; }
    public int ClassificationID { get; set; }
    public string ClassificationName { get; set; } = string.Empty;
    public int ClassificationSystemID { get; set; }
    public string ClassificationSystemName { get; set; } = string.Empty;
    public string? ProjectClassificationNotes { get; set; }
}

/// <summary>
/// Request DTO for saving the Classifications step.
/// </summary>
public class ProjectClassificationsStepRequestDto
{
    public List<ProjectClassificationRequestItem> Classifications { get; set; } = new();
}

/// <summary>
/// Request item for a single classification assignment.
/// </summary>
public class ProjectClassificationRequestItem
{
    public int? ProjectClassificationID { get; set; }
    public int ClassificationID { get; set; }
    public string? ProjectClassificationNotes { get; set; }
}
