namespace WADNR.Models.DataTransferObjects;

public class ProjectFactSheet
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectDescription { get; set; } = string.Empty;
    public ProjectTypeLookupItemWithColor ProjectType { get; set; }
    public OrganizationLookupItem LeadImplementer { get; set; }
    public PersonLookupItemWithEmail PrimaryContact { get; set; }
    public ProjectStageLookupItem ProjectStage { get; set; }
    public string Duration { get; set; }

}