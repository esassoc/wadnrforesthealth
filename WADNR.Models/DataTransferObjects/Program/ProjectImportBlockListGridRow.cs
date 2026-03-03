namespace WADNR.Models.DataTransferObjects;

public class ProjectImportBlockListGridRow
{
    public int ProjectImportBlockListID { get; set; }
    public int ProgramID { get; set; }
    public int? ProjectID { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectGisIdentifier { get; set; }
    public string? Notes { get; set; }
}
