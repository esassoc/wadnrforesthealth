namespace WADNR.Models.DataTransferObjects;

public class ProjectExternalLinkGridRow
{
    public int ProjectExternalLinkID { get; set; }
    public string ExternalLinkLabel { get; set; } = string.Empty;
    public string ExternalLinkUrl { get; set; } = string.Empty;
}
