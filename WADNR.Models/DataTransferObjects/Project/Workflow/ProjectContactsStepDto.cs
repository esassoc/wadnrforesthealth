namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Contacts step of the Project Create workflow.
/// Note: Available relationship types should be fetched from GET /lookups/person-relationship-types
/// Note: Available people should be fetched from GET /persons/lookup
/// </summary>
public class ProjectContactsStep
{
    public int ProjectID { get; set; }
    /// <summary>
    /// Currently assigned contacts for the project.
    /// </summary>
    public List<ProjectContactStepItem> Contacts { get; set; } = new();
}

/// <summary>
/// Item representing a contact/person assigned to a project.
/// </summary>
public class ProjectContactStepItem
{
    public int? ProjectPersonID { get; set; }
    public int PersonID { get; set; }
    public string PersonFullName { get; set; } = string.Empty;
    public int ProjectPersonRelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
}

/// <summary>
/// Request for saving the Contacts step.
/// </summary>
public class ProjectContactsStepRequest
{
    public List<ProjectContactRequestItem> Contacts { get; set; } = new();
}

/// <summary>
/// Request item for a single contact assignment.
/// </summary>
public class ProjectContactRequestItem
{
    public int? ProjectPersonID { get; set; }
    public int PersonID { get; set; }
    public int ProjectPersonRelationshipTypeID { get; set; }
}
