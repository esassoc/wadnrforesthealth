namespace WADNR.Models.DataTransferObjects;

public class ProjectClassificationSaveRequest
{
    public List<ProjectClassificationItemRequest> Classifications { get; set; } = new();
}

public class ProjectClassificationItemRequest
{
    public int? ProjectClassificationID { get; set; }
    public int ClassificationID { get; set; }
    public string? ProjectClassificationNotes { get; set; }
}
