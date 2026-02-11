namespace WADNR.Models.DataTransferObjects;

public class OrganizationGridRow
{
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationShortName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string OrganizationTypeName { get; set; } = string.Empty;
    public int AssociatedProjectsCount { get; set; }
    public int AssociatedFundSourcesCount { get; set; }
    public int AssociatedUsersCount { get; set; }
    public string? PrimaryContactPersonFullName { get; set; }
    public bool CanStewardProjects { get; set; }
}
