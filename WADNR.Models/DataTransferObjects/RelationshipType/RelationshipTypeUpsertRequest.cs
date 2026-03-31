namespace WADNR.Models.DataTransferObjects;

public class RelationshipTypeUpsertRequest
{
    public string RelationshipTypeName { get; set; } = string.Empty;
    public string? RelationshipTypeDescription { get; set; }
    public bool CanStewardProjects { get; set; }
    public bool IsPrimaryContact { get; set; }
    public bool CanOnlyBeRelatedOnceToAProject { get; set; }
    public bool ShowOnFactSheet { get; set; }
    public bool ReportInAccomplishmentsDashboard { get; set; }
    public List<int> OrganizationTypeIDs { get; set; } = new();
}
