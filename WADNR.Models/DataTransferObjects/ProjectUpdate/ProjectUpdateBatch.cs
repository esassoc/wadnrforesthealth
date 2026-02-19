namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Detail DTO for a Project Update batch (versioned editing session for approved projects).
/// </summary>
public class ProjectUpdateBatchDetail
{
    public int ProjectUpdateBatchID { get; set; }
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int ProjectUpdateStateID { get; set; }
    public string ProjectUpdateStateName { get; set; } = string.Empty;
    public DateTime LastUpdateDate { get; set; }
    public string? LastUpdatedByPersonName { get; set; }
    public string? SubmittedByPersonName { get; set; }
    public DateTime? SubmittalDate { get; set; }
    public string? ApprovedByPersonName { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ReturnedByPersonName { get; set; }
    public string? ReturnComment { get; set; }

    // User permission flags
    public bool CanEdit { get; set; }
    public bool CanSubmit { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReturn { get; set; }
    public bool CanDelete { get; set; }
}

/// <summary>
/// Summary of an Update batch for grid display.
/// </summary>
public class ProjectUpdateBatchGridRow
{
    public int ProjectUpdateBatchID { get; set; }
    public int ProjectID { get; set; }
    public int ProjectUpdateStateID { get; set; }
    public string ProjectUpdateStateName { get; set; } = string.Empty;
    public DateTime LastUpdateDate { get; set; }
    public string? LastUpdatedByPersonName { get; set; }
    public string? SubmittedByPersonName { get; set; }
    public DateTime? SubmittalDate { get; set; }
    public string? ApprovedByPersonName { get; set; }
    public DateTime? ApprovalDate { get; set; }
}

/// <summary>
/// Request for returning an Update batch with per-section reviewer comments.
/// </summary>
public class ProjectUpdateReturnRequest
{
    public string? BasicsComment { get; set; }
    public string? LocationSimpleComment { get; set; }
    public string? LocationDetailedComment { get; set; }
    public string? ExpectedFundingComment { get; set; }
    public string? ContactsComment { get; set; }
    public string? OrganizationsComment { get; set; }
}
