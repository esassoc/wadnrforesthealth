namespace WADNR.Models.DataTransferObjects;

public class ReportTemplateGridRow
{
    public int ReportTemplateID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ReportTemplateModelID { get; set; }
    public string ReportTemplateModelDisplayName { get; set; } = string.Empty;
    public bool IsSystemTemplate { get; set; }
    public string FileResourceGuid { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
}
