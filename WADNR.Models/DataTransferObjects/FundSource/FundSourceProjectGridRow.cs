namespace WADNR.Models.DataTransferObjects;

public class FundSourceProjectGridRow
{
    public int FundSourceAllocationID { get; set; }
    public string? FundSourceAllocationName { get; set; }
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? FhtProjectNumber { get; set; }
    public string? ProjectStageName { get; set; }
    public OrganizationLookupItem? LeadImplementer { get; set; }
    public ProjectTypeLookupItem? ProjectType { get; set; }
    public List<CountyLookupItem> Counties { get; set; } = new();
    public List<PriorityLandscapeLookupItem> PriorityLandscapes { get; set; } = new();
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
