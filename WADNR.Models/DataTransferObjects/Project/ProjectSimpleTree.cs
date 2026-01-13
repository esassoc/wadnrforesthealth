namespace WADNR.Models.DataTransferObjects;

public class ProjectSimpleTree
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; }
    public ProjectTypeLookupItem ProjectType { get; set; }
}