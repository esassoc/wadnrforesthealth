namespace WADNR.Models.DataTransferObjects;

public class TaxonomyBranchDetail
{
    public int TaxonomyBranchID { get; set; }
    public string TaxonomyBranchName { get; set; } = string.Empty;
    public string? TaxonomyBranchDescription { get; set; }
    public string? TaxonomyBranchCode { get; set; }
    public string? ThemeColor { get; set; }
    public int? TaxonomyBranchSortOrder { get; set; }
    public TaxonomyTrunkLookupItem TaxonomyTrunk { get; set; } = null!;
    public List<ProjectTypeLookupItem> ProjectTypes { get; set; } = new();
    public int ProjectCount { get; set; }
    public bool HasProjects { get; set; }
}
