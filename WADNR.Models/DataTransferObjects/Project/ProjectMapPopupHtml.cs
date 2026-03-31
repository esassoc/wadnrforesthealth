using System.Collections.Generic;

namespace WADNR.Models.DataTransferObjects;

public class ProjectMapPopupHtml
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Duration { get; set; }
    public string ProjectTypeName { get; set; } = string.Empty;
    public string ProjectStageName { get; set; } = string.Empty;
    public int? LeadImplementerOrganizationID { get; set; }
    public string? LeadImplementerName { get; set; }
    public List<ClassificationWithSystemLookupItem> Classifications { get; set; } = new();
}
