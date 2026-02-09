using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class ProjectContactSaveRequest
{
    public List<ProjectContactItemRequest> Contacts { get; set; } = new();
}

public class ProjectContactItemRequest
{
    public int? ProjectPersonID { get; set; }
    [Required] public int PersonID { get; set; }
    [Required] public int ProjectPersonRelationshipTypeID { get; set; }
}
