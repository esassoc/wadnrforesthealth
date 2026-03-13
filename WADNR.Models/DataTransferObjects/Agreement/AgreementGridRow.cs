namespace WADNR.Models.DataTransferObjects;

public class AgreementGridRow
{
    public int AgreementID { get; set; }
    public string AgreementTitle { get; set; } = string.Empty;
    public string? AgreementNumber { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? AgreementAmount { get; set; }
    public decimal? ExpendedAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
    public string? AgreementTypeAbbrev { get; set; }
    public string? AgreementStatusName { get; set; }
    public OrganizationLookupItem? Organization { get; set; }
    public List<FundSourceLookupItem> FundSources { get; set; } = new();
    public string ProgramIndices { get; set; } = string.Empty;
    public string ProjectCodes { get; set; } = string.Empty;
}