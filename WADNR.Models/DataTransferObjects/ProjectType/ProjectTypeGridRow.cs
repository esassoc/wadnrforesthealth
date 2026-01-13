namespace WADNR.Models.DataTransferObjects;

public class ProjectTypeGridRow
{
    public int ProjectTypeID { get; set; }
    public string ProjectTypeName { get; set; } = string.Empty;
    public string? ProjectTypeCode { get; set; }
    public int? ProjectTypeSortOrder { get; set; }
    public bool LimitVisibilityToAdmin { get; set; }
}