namespace WADNR.Models.DataTransferObjects;

public class ProjectUpsertRequest
{
    public int ProjectTypeID { get; set; }
    public int ProjectStageID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public int ProjectApprovalStatusID { get; set; }
    public int? ProposingPersonID { get; set; }
    public DateTimeOffset? ProposingDate { get; set; }
    public DateTimeOffset? SubmissionDate { get; set; }
    public DateTimeOffset? ApprovalDate { get; set; }
    public int? FocusAreaID { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public string FhtProjectNumber { get; set; } = string.Empty;
}
