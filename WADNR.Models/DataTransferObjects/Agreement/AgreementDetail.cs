namespace WADNR.Models.DataTransferObjects;

public class AgreementDetail
{
    public int AgreementID { get; set; }
    public int AgreementTypeID { get; set; }
    public string AgreementTitle { get; set; } = string.Empty;
    public string? AgreementNumber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? AgreementAmount { get; set; }
    public decimal? ExpendedAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
    public int? DNRUplandRegionID { get; set; }
    public DateTime? FirstBillDueOn { get; set; }
    public string? Notes { get; set; }
    public int OrganizationID { get; set; }
    public int? AgreementStatusID { get; set; }
    public int? AgreementFileResourceID { get; set; }

    public string? AgreementTypeName { get; set; }
    public string? AgreementStatusName { get; set; }
    public string? OrganizationName { get; set; }
    public string? DNRUplandRegionName { get; set; }
}