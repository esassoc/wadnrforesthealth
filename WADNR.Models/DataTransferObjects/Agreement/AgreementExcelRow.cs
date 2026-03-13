namespace WADNR.Models.DataTransferObjects;

public class AgreementExcelRow
{
    public string? AgreementTypeAbbrev { get; set; }
    public string? AgreementNumber { get; set; }
    public string FundSourceAllocationNumbers { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string AgreementTitle { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? AgreementAmount { get; set; }
}
