namespace WADNR.Models.DataTransferObjects;

public class FundSourceUpsertRequest
{
    public string FundSourceName { get; set; } = string.Empty;
    public string? FundSourceNumber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ConditionsAndRequirements { get; set; }
    public string? ComplianceNotes { get; set; }
    public string? CFDANumber { get; set; }
    public int? FundSourceTypeID { get; set; }
    public string? ShortName { get; set; }
    public int FundSourceStatusID { get; set; }
    public int OrganizationID { get; set; }
    public decimal TotalAwardAmount { get; set; }
}
