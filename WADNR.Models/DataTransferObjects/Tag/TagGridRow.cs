namespace WADNR.Models.DataTransferObjects;

public class TagGridRow
{
    public int TagID { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? TagDescription { get; set; }
    public int ProjectCount { get; set; }
}
