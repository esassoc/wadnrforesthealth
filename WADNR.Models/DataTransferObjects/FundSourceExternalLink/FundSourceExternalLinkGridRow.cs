namespace WADNR.Models.DataTransferObjects;

public class FundSourceExternalLinkGridRow
{
    public int FundSourceExternalLinkID { get; set; }
    public string ExternalLinkLabel { get; set; } = string.Empty;
    public string ExternalLinkUrl { get; set; } = string.Empty;
}
