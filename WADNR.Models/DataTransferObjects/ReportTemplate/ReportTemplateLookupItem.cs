namespace WADNR.Models.DataTransferObjects;

public class ReportTemplateLookupItem
{
    public int ReportTemplateID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int ReportTemplateModelID { get; set; }
}
