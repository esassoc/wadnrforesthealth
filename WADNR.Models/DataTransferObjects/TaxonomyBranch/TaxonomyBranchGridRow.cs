namespace WADNR.Models.DataTransferObjects;

public class TaxonomyBranchGridRow
{
    public int TaxonomyBranchID { get; set; }
    public string TaxonomyBranchName { get; set; } = string.Empty;
    public string? TaxonomyBranchCode { get; set; }
    public int? TaxonomyBranchSortOrder { get; set; }
    public TaxonomyTrunkLookupItem TaxonomyTrunk { get; set; } = null!;
    public int ProjectTypeCount { get; set; }
    public int ProjectCount { get; set; }
}
