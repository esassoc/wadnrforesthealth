namespace WADNR.Models.DataTransferObjects;

public class ProjectBasicsSaveRequest
{
    public int ProjectTypeID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public int ProjectStageID { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public string? ProjectGisIdentifier { get; set; }
    public int? LeadImplementerOrganizationID { get; set; }
    public int? FocusAreaID { get; set; }
    public int? PercentageMatch { get; set; }
    public List<int> ProgramIDs { get; set; } = new();
}
