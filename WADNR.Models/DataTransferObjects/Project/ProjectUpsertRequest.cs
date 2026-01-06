namespace WADNR.Models.DataTransferObjects;

public class ProjectUpsertRequest
{
    public int ProjectTypeID { get; set; }
    public int ProjectStageID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public int ProjectApprovalStatusID { get; set; }
    public int? ProposingPersonID { get; set; }
    public DateTime? ProposingDate { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public int? FocusAreaID { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string FhtProjectNumber { get; set; } = string.Empty;
}
