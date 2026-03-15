namespace WADNR.Models.DataTransferObjects;

public class CustomPageMenuItem
{
    public int CustomPageID { get; set; }
    public string CustomPageDisplayName { get; set; } = string.Empty;
    public string CustomPageVanityUrl { get; set; } = string.Empty;
    public int CustomPageDisplayTypeID { get; set; }
}
