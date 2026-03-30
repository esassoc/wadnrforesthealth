namespace WADNR.Models.DataTransferObjects;

public class RelationshipTypeGridRow
{
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public bool CanStewardProjects { get; set; }
    public bool IsPrimaryContact { get; set; }
    public bool CanOnlyBeRelatedOnceToAProject { get; set; }
    public bool ShowOnFactSheet { get; set; }
    public bool ReportInAccomplishmentsDashboard { get; set; }
    public string? RelationshipTypeDescription { get; set; }
    public List<string> AssociatedOrganizationTypeNames { get; set; } = new();
    public int ProjectOrganizationCount { get; set; }
}
