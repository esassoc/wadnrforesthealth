namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// DTO for the Organizations step of the ProjectCreate wizard.
/// Note: Available relationship types should be fetched from GET /lookups/organization-relationship-types
/// Note: Available organizations should be fetched from GET /organizations/lookup
/// </summary>
public class ProjectOrganizationsStepDto
{
    public int ProjectID { get; set; }
    /// <summary>
    /// Currently assigned organizations for the project.
    /// </summary>
    public List<ProjectOrganizationStepItem> Organizations { get; set; } = new();
}

/// <summary>
/// Item representing an organization assigned to a project.
/// </summary>
public class ProjectOrganizationStepItem
{
    public int? ProjectOrganizationID { get; set; }
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public bool IsPrimaryContact { get; set; }
}

/// <summary>
/// Request DTO for saving the Organizations step.
/// </summary>
public class ProjectOrganizationsStepRequestDto
{
    public List<ProjectOrganizationRequestItem> Organizations { get; set; } = new();
}

/// <summary>
/// Request item for a single organization assignment.
/// </summary>
public class ProjectOrganizationRequestItem
{
    public int? ProjectOrganizationID { get; set; }
    public int OrganizationID { get; set; }
    public int RelationshipTypeID { get; set; }
}
