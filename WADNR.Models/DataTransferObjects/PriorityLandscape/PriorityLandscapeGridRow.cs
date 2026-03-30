namespace WADNR.Models.DataTransferObjects;

public class PriorityLandscapeGridRow
{
    public int PriorityLandscapeID { get; set; }
    public string PriorityLandscapeName { get; set; } = string.Empty;
    public int? PlanYear { get; set; }
    public string? PriorityLandscapeCategoryName { get; set; }
    public int ProjectCount { get; set; }
}
