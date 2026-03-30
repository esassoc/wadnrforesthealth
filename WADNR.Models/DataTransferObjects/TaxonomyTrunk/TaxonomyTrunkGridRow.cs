namespace WADNR.Models.DataTransferObjects;

public class TaxonomyTrunkGridRow
{
    public int TaxonomyTrunkID { get; set; }
    public string TaxonomyTrunkName { get; set; } = string.Empty;
    public string? TaxonomyTrunkCode { get; set; }
    public int? TaxonomyTrunkSortOrder { get; set; }
    public int TaxonomyBranchCount { get; set; }
    public int ProjectCount { get; set; }
    public List<TaxonomyBranchLookupItem> TaxonomyBranches { get; set; } = new();
}
