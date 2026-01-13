namespace WADNR.Models.DataTransferObjects;

public class ProjectTypeTaxonomy
{
    public int ProjectTypeID { get; set; }
    public string ProjectTypeName { get; set; } = string.Empty;
    public List<ProjectLookupItem> Projects { get; set; } = new();
}