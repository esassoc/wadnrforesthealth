namespace WADNR.Models.DataTransferObjects;

public class ProjectDescriptionExcelRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
}
