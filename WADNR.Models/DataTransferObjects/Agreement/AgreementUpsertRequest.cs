namespace WADNR.Models.DataTransferObjects;

public class AgreementUpsertRequest
{
    public int AgreementTypeID { get; set; }
    public string AgreementTitle { get; set; } = string.Empty;
    public string? AgreementNumber { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? AgreementAmount { get; set; }
    public decimal? ExpendedAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
    public int? DNRUplandRegionID { get; set; }
    public DateOnly? FirstBillDueOn { get; set; }
    public string? Notes { get; set; }
    public int OrganizationID { get; set; }
    public int? AgreementStatusID { get; set; }
    public int? AgreementFileResourceID { get; set; }
}