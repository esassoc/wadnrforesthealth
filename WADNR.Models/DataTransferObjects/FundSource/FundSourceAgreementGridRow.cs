namespace WADNR.Models.DataTransferObjects;

public class FundSourceAgreementGridRow
{
    public int AgreementID { get; set; }
    public string AgreementTitle { get; set; } = string.Empty;
    public string? AgreementNumber { get; set; }
    public string? AgreementTypeAbbrev { get; set; }
    public int? OrganizationID { get; set; }
    public string? OrganizationName { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? AgreementAmount { get; set; }
    public string? ProgramIndices { get; set; }
    public string? ProjectCodes { get; set; }
}
