namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Lookup item for organization relationship types (used in project organizations).
/// </summary>
public class OrganizationRelationshipTypeLookupItem
{
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public string? RelationshipTypeDescription { get; set; }
    public bool CanOnlyBeRelatedOnceToAProject { get; set; }
    public bool IsPrimaryContact { get; set; }
    public int SortOrder { get; set; }
}
