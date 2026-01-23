namespace WADNR.Models.DataTransferObjects.ProjectCode;

public class ProjectCodeGridRow
{
    public int ProjectCodeID { get; set; }
    public string ProjectCodeName { get; set; } = string.Empty;
    public string? ProjectCodeTitle { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ProjectStartDate { get; set; }
    public DateTime? ProjectEndDate { get; set; }
    public int InvoiceCount { get; set; }
}
