namespace WADNR.Models.DataTransferObjects;

public class ProjectClassificationExcelRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ClassificationName { get; set; } = string.Empty;
}
