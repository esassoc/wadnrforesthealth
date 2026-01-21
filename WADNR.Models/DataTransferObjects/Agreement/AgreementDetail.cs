namespace WADNR.Models.DataTransferObjects;

public class AgreementDetail
{
    public int AgreementID { get; set; }
    public AgreementTypeLookupItem AgreementType { get; set; } = new();
    public string AgreementTitle { get; set; } = string.Empty;
    public string? AgreementNumber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? AgreementAmount { get; set; }
    public decimal? ExpendedAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
    public DNRUplandRegionLookupItem? DNRUplandRegion { get; set; }
    public DateTime? FirstBillDueOn { get; set; }
    public string? Notes { get; set; }
    public OrganizationLookupItem ContributingOrganization { get; set; } = new();
    public AgreementStatusLookupItem? AgreementStatus { get; set; }
    public FileResourceLookupItem? FileResource { get; set; }

    public string ProgramIndices { get; set; } = string.Empty;
    public string ProjectCodes { get; set; } = string.Empty;
}