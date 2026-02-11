namespace WADNR.Models.DataTransferObjects;

public class ProjectOrganizationItem
{
    public int ProjectOrganizationID { get; set; }
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public bool IsPrimaryContact { get; set; }
}
