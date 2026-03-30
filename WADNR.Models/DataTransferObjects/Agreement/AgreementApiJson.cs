using System;

namespace WADNR.Models.DataTransferObjects.Agreement;

public class AgreementApiJson
{
    public int AgreementID { get; set; }
    public int AgreementTypeID { get; set; }
    public string AgreementTypeName { get; set; }
    public string AgreementNumber { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? AgreementAmount { get; set; }
    public decimal? ExpendedAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
    public int? RegionID { get; set; }
    public string RegionName { get; set; }
    public DateOnly? FirstBillDueOn { get; set; }
    public string Notes { get; set; }
    public string AgreementTitle { get; set; }
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; }
    public int? AgreementStatusID { get; set; }
    public string AgreementStatusName { get; set; }
    public int? AgreementFileResourceID { get; set; }
}
