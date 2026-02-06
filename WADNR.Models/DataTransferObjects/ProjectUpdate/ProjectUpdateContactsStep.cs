namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Contacts step of the Project Update workflow.
/// </summary>
public class ProjectUpdateContactsStep
{
    public int ProjectUpdateBatchID { get; set; }
    /// <summary>
    /// Currently assigned contacts for the update batch.
    /// </summary>
    public List<ProjectPersonUpdateItem> Contacts { get; set; } = new();
}

/// <summary>
/// Item representing a contact/person assigned to a project update.
/// </summary>
public class ProjectPersonUpdateItem
{
    public int? ProjectPersonUpdateID { get; set; }
    public int PersonID { get; set; }
    public string PersonFullName { get; set; } = string.Empty;
    public int ProjectPersonRelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
}

/// <summary>
/// Request for saving the Contacts step of the Project Update workflow.
/// </summary>
public class ProjectUpdateContactsStepRequest
{
    public List<ProjectPersonUpdateItemRequest> Contacts { get; set; } = new();
}

/// <summary>
/// Request item for a single contact assignment in the update.
/// </summary>
public class ProjectPersonUpdateItemRequest
{
    public int? ProjectPersonUpdateID { get; set; }
    public int PersonID { get; set; }
    public int ProjectPersonRelationshipTypeID { get; set; }
}
