namespace WADNR.Models.DataTransferObjects;

public class CustomPageUpsertRequest
{
    public string CustomPageDisplayName { get; set; } = string.Empty;
    public string CustomPageVanityUrl { get; set; } = string.Empty;
    public int CustomPageDisplayTypeID { get; set; }
    public int CustomPageNavigationSectionID { get; set; }
}
