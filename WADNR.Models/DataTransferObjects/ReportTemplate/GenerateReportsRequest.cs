namespace WADNR.Models.DataTransferObjects;

public class GenerateReportsRequest
{
    public int ReportTemplateID { get; set; }
    public List<int> ModelIDList { get; set; } = new();
}
