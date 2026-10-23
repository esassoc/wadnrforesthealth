using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class FundSourceExternalLinkSaveRequest
{
    public List<FundSourceExternalLinkItemRequest> ExternalLinks { get; set; } = new();
}

public class FundSourceExternalLinkItemRequest
{
    public int? FundSourceExternalLinkID { get; set; }
    [Required, MaxLength(300)] public string ExternalLinkLabel { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string ExternalLinkUrl { get; set; } = string.Empty;
}
