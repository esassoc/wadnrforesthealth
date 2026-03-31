using System;
using System.Collections.Generic;

namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationApiJson
{
    public int FundSourceAllocationID { get; set; }
    public string FundSourceAllocationName { get; set; }
    public int FundSourceID { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? AllocationAmount { get; set; }
    public int? FederalFundCodeID { get; set; }
    public string FederalFundCodeName { get; set; }
    public int? OrganizationID { get; set; }
    public string OrganizationName { get; set; }
    public int? RegionID { get; set; }
    public string RegionName { get; set; }
    public int? DivisionID { get; set; }
    public string DivisionName { get; set; }
    public int? FundSourceManagerID { get; set; }
    public string FundSourceManagerName { get; set; }
    public List<int> FundSourceAllocationFileResourceIDs { get; set; }
}
