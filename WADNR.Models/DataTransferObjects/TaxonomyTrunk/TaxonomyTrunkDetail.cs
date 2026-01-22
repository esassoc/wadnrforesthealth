namespace WADNR.Models.DataTransferObjects;

public class TaxonomyTrunkDetail
{
    public int TaxonomyTrunkID { get; set; }
    public string TaxonomyTrunkName { get; set; } = string.Empty;
    public string? TaxonomyTrunkDescription { get; set; }
    public string? TaxonomyTrunkCode { get; set; }
    public string? ThemeColor { get; set; }
    public int? TaxonomyTrunkSortOrder { get; set; }
    public List<TaxonomyBranchLookupItem> TaxonomyBranches { get; set; } = new();
    public List<ProjectTypeLookupItem> ProjectTypes { get; set; } = new();
    public int ProjectCount { get; set; }
    public bool HasProjects { get; set; }
}
