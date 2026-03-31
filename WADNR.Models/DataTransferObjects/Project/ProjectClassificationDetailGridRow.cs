using System;

namespace WADNR.Models.DataTransferObjects;

public class ProjectClassificationDetailGridRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string FhtProjectNumber { get; set; } = string.Empty;
    public OrganizationLookupItem? PrimaryContactOrganization { get; set; }
    public ProjectStageLookupItem ProjectStage { get; set; } = null!;
    public DateOnly? ProjectInitiationDate { get; set; }
    public string ProjectThemeNotes { get; set; } = string.Empty;
}
