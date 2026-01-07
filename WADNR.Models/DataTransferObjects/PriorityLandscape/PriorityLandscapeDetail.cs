using WADNR.Models.DataTransferObjects.PriorityLandscape;

namespace WADNR.Models.DataTransferObjects;

public class PriorityLandscapeDetail
{
    public int PriorityLandscapeID { get; set; }
    public string PriorityLandscapeName { get; set; } = string.Empty;
    public string? PriorityLandscapeDescription { get; set; }
    public PriorityLandscapeCategoryLookupItem PriorityLandscapeCategory { get; set; } = new PriorityLandscapeCategoryLookupItem();
    public string? PriorityLandscapeExternalResources { get; set; }
    public string? PriorityLandscapeAboveMapText { get; set; }
}
