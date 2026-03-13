namespace WADNR.Models.DataTransferObjects;
public class ProjectDNRUplandRegionDetailGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public OrganizationLookupItem? LeadImplementer { get; set; }
    public List<ProgramLookupItem> Programs { get; set; } = new();
    public List<CountyLookupItem> Counties { get; set; } = new();
    public PersonLookupItem? PrimaryContact { get; set; }
    public decimal? TotalTreatedAcres { get; set; }
    public ProjectTypeLookupItem ProjectType { get; set; } = new();
    public ProjectStageLookupItem ProjectStage { get; set; } = new();
    public DateTimeOffset? ProjectApplicationDate { get; set; }
    public DateOnly? ProjectInitiationDate { get; set; }
    public DateOnly? ProjectExpiryDate { get; set; }
    public DateOnly? ProjectCompletionDate { get; set; }
    public decimal TotalPaymentAmount { get; set; }
    public decimal TotalMatchAmount { get; set; }
    public int? PercentageMatch { get; set; }
    public List<FundSourceAllocationLookupItem> ExpectedFundingFundSourceAllocations { get; set; } = new();
    public string? PrivateLandowners { get; set; }
    public List<TagLookupItem> Tags { get; set; } = new();
}
