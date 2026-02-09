using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class ProjectOrganizationSaveRequest
{
    public List<ProjectOrganizationItemRequest> Organizations { get; set; } = new();
}

public class ProjectOrganizationItemRequest
{
    public int? ProjectOrganizationID { get; set; }
    [Required] public int OrganizationID { get; set; }
    [Required] public int RelationshipTypeID { get; set; }
}
