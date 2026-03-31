namespace WADNR.Models.DataTransferObjects;

public class ProjectOrganizationExcelRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string RelationshipTypeName { get; set; } = string.Empty;
    public string? PrimaryContactPersonName { get; set; }
    public string? OrganizationTypeName { get; set; }
}
