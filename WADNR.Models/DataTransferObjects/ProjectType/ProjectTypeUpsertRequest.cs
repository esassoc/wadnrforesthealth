namespace WADNR.Models.DataTransferObjects;

public class ProjectTypeUpsertRequest
{
    public int? TaxonomyBranchID { get; set; }
    public string ProjectTypeName { get; set; } = string.Empty;
    public string? ProjectTypeDescription { get; set; }
    public string? ProjectTypeCode { get; set; }
    public string? ThemeColor { get; set; }
    public int? ProjectTypeSortOrder { get; set; }
    public bool LimitVisibilityToAdmin { get; set; }
}
