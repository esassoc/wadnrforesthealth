using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class TaxonomyTrunkUpsertRequest
{
    [Required]
    [StringLength(100)]
    public string TaxonomyTrunkName { get; set; } = string.Empty;

    public string? TaxonomyTrunkDescription { get; set; }

    [StringLength(10)]
    public string? TaxonomyTrunkCode { get; set; }

    [StringLength(20)]
    public string? ThemeColor { get; set; }

    public int? TaxonomyTrunkSortOrder { get; set; }
}
