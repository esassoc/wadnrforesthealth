using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class ProjectExternalLinkSaveRequest
{
    public List<ProjectExternalLinkItemRequest> ExternalLinks { get; set; } = new();
}

public class ProjectExternalLinkItemRequest
{
    public int? ProjectExternalLinkID { get; set; }
    [Required, MaxLength(300)] public string ExternalLinkLabel { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string ExternalLinkUrl { get; set; } = string.Empty;
}
