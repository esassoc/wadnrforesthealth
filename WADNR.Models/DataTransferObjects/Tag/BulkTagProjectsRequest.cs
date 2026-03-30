namespace WADNR.Models.DataTransferObjects;

public class BulkTagProjectsRequest
{
    public string TagName { get; set; } = string.Empty;
    public List<int> ProjectIDs { get; set; } = new();
}
