namespace WADNR.Models.DataTransferObjects;

public class ProjectGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string FhtProjectNumber { get; set; } = string.Empty;
    public ProjectTypeLookupItem ProjectType { get; set; }
    public ProjectStageLookupItem ProjectStage { get; set; }
    public decimal? TotalTreatedAcres { get; set; }
    public OrganizationLookupItem? LeadImplementerOrganization { get; set; }
    public List<ProgramLookupItem> Programs { get; set; } = new List<ProgramLookupItem>();
    public PriorityLandscapeLookupItem? PriorityLandscape { get; set; }
    public CountyLookupItem? County { get; set; }
}
