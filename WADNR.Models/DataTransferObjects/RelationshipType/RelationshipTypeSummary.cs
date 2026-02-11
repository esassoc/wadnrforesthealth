namespace WADNR.Models.DataTransferObjects;

public class RelationshipTypeSummary
{
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public string? RelationshipTypeDescription { get; set; }
    public bool CanStewardProjects { get; set; }
    public bool IsPrimaryContact { get; set; }
    public bool CanOnlyBeRelatedOnceToAProject { get; set; }
}
