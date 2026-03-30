namespace WADNR.Models.DataTransferObjects;

public class ProjectProgramDetailGridRow
{
    public int ProjectID { get; set; }
    public string? ProjectGisIdentifier { get; set; }
    public string FhtProjectNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectTypeName { get; set; }
    public ProjectStageLookupItem ProjectStage { get; set; } = null!;
    public string Programs { get; set; } = string.Empty;
}
