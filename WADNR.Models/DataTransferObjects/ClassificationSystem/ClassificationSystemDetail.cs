namespace WADNR.Models.DataTransferObjects.ClassificationSystem;

public class ClassificationSystemDetail
{
    public int ClassificationSystemID { get; set; }
    public string ClassificationSystemName { get; set; } = string.Empty;
    public string? ClassificationSystemDefinition { get; set; }
    public string? ClassificationSystemListPageContent { get; set; }
    public int ClassificationCount { get; set; }
}
