namespace WADNR.Models.DataTransferObjects;

public class ProjectDocumentGridRow
{
    public int ProjectDocumentID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DocumentTypeName { get; set; }
    public int FileResourceID { get; set; }
    public string FileResourceGuid { get; set; } = string.Empty;
}
