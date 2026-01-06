namespace WADNR.Models.DataTransferObjects;

public class ProjectDetail
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public string FhtProjectNumber { get; set; } = string.Empty;
}
