namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Organizations step of the Project Update workflow.
/// </summary>
public class ProjectUpdateOrganizationsStep
{
    public int ProjectUpdateBatchID { get; set; }
    /// <summary>
    /// Currently assigned organizations for the update batch.
    /// </summary>
    public List<ProjectOrganizationUpdateItem> Organizations { get; set; } = new();
}

/// <summary>
/// Item representing an organization assigned to a project update.
/// </summary>
public class ProjectOrganizationUpdateItem
{
    public int? ProjectOrganizationUpdateID { get; set; }
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public bool IsPrimaryContact { get; set; }
}

/// <summary>
/// Request for saving the Organizations step of the Project Update workflow.
/// </summary>
public class ProjectUpdateOrganizationsStepRequest
{
    public List<ProjectOrganizationUpdateItemRequest> Organizations { get; set; } = new();
}

/// <summary>
/// Request item for a single organization assignment in the update.
/// </summary>
public class ProjectOrganizationUpdateItemRequest
{
    public int? ProjectOrganizationUpdateID { get; set; }
    public int OrganizationID { get; set; }
    public int RelationshipTypeID { get; set; }
}
