namespace WADNR.Models.DataTransferObjects;

public class ProjectSearchResult
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectStageName { get; set; }
    public string? ProjectTypeName { get; set; }
}
