using System.Collections.Generic;

namespace WADNR.Models.DataTransferObjects;

public class ProjectMapPopup
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Duration { get; set; }
    public ProjectTypeLookupItem ProjectType { get; set; } = new ProjectTypeLookupItem();
    public ProjectStageLookupItem ProjectStage { get; set; } = new ProjectStageLookupItem();
    public OrganizationLookupItem? LeadImplementer { get; set; }
    public List<ClassificationLookupItem> Classifications { get; set; } = new();
}
