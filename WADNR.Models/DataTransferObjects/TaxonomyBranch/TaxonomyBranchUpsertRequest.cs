using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class TaxonomyBranchUpsertRequest
{
    [Required]
    public int TaxonomyTrunkID { get; set; }

    [Required]
    [StringLength(100)]
    public string TaxonomyBranchName { get; set; } = string.Empty;

    public string? TaxonomyBranchDescription { get; set; }

    [StringLength(10)]
    public string? TaxonomyBranchCode { get; set; }

    [StringLength(20)]
    public string? ThemeColor { get; set; }

    public int? TaxonomyBranchSortOrder { get; set; }
}
