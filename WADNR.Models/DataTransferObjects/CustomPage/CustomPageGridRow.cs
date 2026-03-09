namespace WADNR.Models.DataTransferObjects;

public class CustomPageGridRow
{
    public int CustomPageID { get; set; }
    public string CustomPageDisplayName { get; set; }
    public string CustomPageVanityUrl { get; set; }
    public int CustomPageDisplayTypeID { get; set; }
    public string CustomPageDisplayTypeName { get; set; }
    public int CustomPageNavigationSectionID { get; set; }
    public string CustomPageNavigationSectionName { get; set; }
    public bool HasContent { get; set; }
    public string? CustomPageContent { get; set; }
}
