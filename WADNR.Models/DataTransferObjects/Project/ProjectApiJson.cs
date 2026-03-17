using System;
using WADNR.Models.DataTransferObjects.Shared;

namespace WADNR.Models.DataTransferObjects.Project;

public class ProjectApiJson
{
    public int ProjectID { get; set; }
    public int ProjectTypeID { get; set; }
    public string ProjectTypeName { get; set; }
    public int ProjectStageID { get; set; }
    public string ProjectStageName { get; set; }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public decimal? EstimatedTotalCost { get; set; }
    public LegacyGeometryWrapper ProjectLocationPoint { get; set; }
    public bool IsFeatured { get; set; }
    public string ProjectLocationNotes { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public int ProjectLocationSimpleTypeID { get; set; }
    public string ProjectLocationSimpleTypeName { get; set; }
    public int? PrimaryContactPersonID { get; set; }
    public string PrimaryContactPersonName { get; set; }
    public int ProjectApprovalStatusID { get; set; }
    public string ProjectApprovalStatusName { get; set; }
    public int? ProposingPersonID { get; set; }
    public string ProposingPersonName { get; set; }
    public DateTime? ProposingDate { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public int? ReviewedByPersonID { get; set; }
    public string ReviewedByPersonName { get; set; }
    public int? FocusAreaID { get; set; }
    public string NoExpendituresToReportExplanation { get; set; }
    public string NoRegionsExplanation { get; set; }
    public string NoPriorityLandscapesExplanation { get; set; }
    public DateOnly? ExpirationDate { get; set; }
}
