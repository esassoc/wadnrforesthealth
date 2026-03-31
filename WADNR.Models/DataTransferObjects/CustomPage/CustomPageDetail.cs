namespace WADNR.Models.DataTransferObjects;

public class CustomPageDetail
{
    public int CustomPageID { get; set; }
    public string CustomPageDisplayName { get; set; } = string.Empty;
    public string CustomPageVanityUrl { get; set; } = string.Empty;
    public int CustomPageDisplayTypeID { get; set; }
    public string? CustomPageContent { get; set; }
    public int CustomPageNavigationSectionID { get; set; }
}
