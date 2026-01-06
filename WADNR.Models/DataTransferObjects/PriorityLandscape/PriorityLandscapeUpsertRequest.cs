namespace WADNR.Models.DataTransferObjects;

public class PriorityLandscapeUpsertRequest
{
    public string PriorityLandscapeName { get; set; } = string.Empty;
    public string? PriorityLandscapeDescription { get; set; }
    public int? PlanYear { get; set; }
}
