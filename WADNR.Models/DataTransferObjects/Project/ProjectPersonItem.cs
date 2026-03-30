namespace WADNR.Models.DataTransferObjects;

public class ProjectPersonItem
{
    public int ProjectPersonID { get; set; }
    public int PersonID { get; set; }
    public string PersonFullName { get; set; } = string.Empty;
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
