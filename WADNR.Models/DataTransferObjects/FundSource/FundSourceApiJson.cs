using System;
using System.Collections.Generic;

namespace WADNR.Models.DataTransferObjects.FundSource;

public class FundSourceApiJson
{
    public int FundSourceID { get; set; }
    public string FundSourceNumber { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string ConditionsAndRequirements { get; set; }
    public string ComplianceNotes { get; set; }
    public decimal? AwardedFunds { get; set; }
    public string CFDANumber { get; set; }
    public string FundSourceName { get; set; }
    public int? FundSourceTypeID { get; set; }
    public string FundSourceTypeName { get; set; }
    public string ShortName { get; set; }
    public int FundSourceStatusID { get; set; }
    public string FundSourceStatusTypeName { get; set; }
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; }
    public List<int> FundSourceFileResourceIDs { get; set; }
}
