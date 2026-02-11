namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Lookup item for person relationship types (used in project contacts).
/// </summary>
public class PersonRelationshipTypeLookupItem
{
    public int ProjectPersonRelationshipTypeID { get; set; }
    public string ProjectPersonRelationshipTypeName { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
}
