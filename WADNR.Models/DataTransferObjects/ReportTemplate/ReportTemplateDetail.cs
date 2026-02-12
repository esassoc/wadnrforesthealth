namespace WADNR.Models.DataTransferObjects;

public class ReportTemplateDetail
{
    public int ReportTemplateID { get; set; }
    public int FileResourceID { get; set; }
    public string FileResourceGuid { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ReportTemplateModelID { get; set; }
    public string ReportTemplateModelDisplayName { get; set; } = string.Empty;
    public int ReportTemplateModelTypeID { get; set; }
    public string ReportTemplateModelTypeDisplayName { get; set; } = string.Empty;
    public bool IsSystemTemplate { get; set; }
}
