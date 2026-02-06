namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the External Links step of the Project Update workflow.
/// </summary>
public class ProjectUpdateExternalLinksStep
{
    public int ProjectUpdateBatchID { get; set; }
    public List<ProjectExternalLinkUpdateItem> ExternalLinks { get; set; } = new();
}

/// <summary>
/// An external link in an Update batch.
/// </summary>
public class ProjectExternalLinkUpdateItem
{
    public int ProjectExternalLinkUpdateID { get; set; }
    public int ProjectUpdateBatchID { get; set; }
    public string ExternalLinkLabel { get; set; } = string.Empty;
    public string ExternalLinkUrl { get; set; } = string.Empty;
}

/// <summary>
/// Request for saving the External Links step of the Project Update workflow.
/// </summary>
public class ProjectUpdateExternalLinksStepRequest
{
    public List<ProjectExternalLinkUpdateItemRequest> ExternalLinks { get; set; } = new();
}

/// <summary>
/// Request item for a single external link in the Update External Links step.
/// </summary>
public class ProjectExternalLinkUpdateItemRequest
{
    public int? ProjectExternalLinkUpdateID { get; set; }
    public string ExternalLinkLabel { get; set; } = string.Empty;
    public string ExternalLinkUrl { get; set; } = string.Empty;
}
