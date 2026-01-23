namespace WADNR.Models.DataTransferObjects;

public class ProjectDocumentDetail
{
    public int ProjectDocumentID { get; set; }
    public int ProjectID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ProjectDocumentTypeID { get; set; }
    public string? ProjectDocumentTypeDisplayName { get; set; }
    public int FileResourceID { get; set; }
    public string FileResourceGuid { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
}
