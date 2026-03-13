namespace WADNR.Models.DataTransferObjects;

public class ProjectFocusAreaDetailGridRow
{
    public int ProjectID { get; set; }
    public string FhtProjectNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public ProjectStageLookupItem ProjectStage { get; set; } = new();
    public DateOnly? ProjectInitiationDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? TotalFunding { get; set; }
    public string? ProjectDescription { get; set; }
    public int PhotoCount { get; set; }
    public List<TagLookupItem> Tags { get; set; } = new();
}
