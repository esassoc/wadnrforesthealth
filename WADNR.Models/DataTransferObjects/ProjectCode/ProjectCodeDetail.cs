namespace WADNR.Models.DataTransferObjects.ProjectCode;

public class ProjectCodeDetail
{
    public int ProjectCodeID { get; set; }
    public string ProjectCodeName { get; set; } = string.Empty;
    public string? ProjectCodeTitle { get; set; }
    public DateTimeOffset? CreateDate { get; set; }
    public DateOnly? ProjectStartDate { get; set; }
    public DateOnly? ProjectEndDate { get; set; }
    public int InvoiceCount { get; set; }
    public int FundSourceAllocationCount { get; set; }
}
