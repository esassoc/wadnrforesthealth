namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationAgreementGridRow
{
    public int AgreementID { get; set; }
    public string? AgreementNumber { get; set; }
    public string AgreementTitle { get; set; } = string.Empty;
    public string? AgreementTypeAbbrev { get; set; }
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? AgreementAmount { get; set; }
}
