namespace WADNR.Models.DataTransferObjects;

public class ProjectClassificationDetailItem
{
    public int ProjectClassificationID { get; set; }
    public int ClassificationID { get; set; }
    public string ClassificationName { get; set; } = string.Empty;
    public int ClassificationSystemID { get; set; }
    public string ClassificationSystemName { get; set; } = string.Empty;
    public string? ProjectClassificationNotes { get; set; }
}
