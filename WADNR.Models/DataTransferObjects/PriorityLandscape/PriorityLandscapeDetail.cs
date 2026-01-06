namespace WADNR.Models.DataTransferObjects;

public class PriorityLandscapeDetail
{
    public int PriorityLandscapeID { get; set; }
    public string PriorityLandscapeName { get; set; } = string.Empty;
    public string? PriorityLandscapeDescription { get; set; }
    public int? PlanYear { get; set; }
}
