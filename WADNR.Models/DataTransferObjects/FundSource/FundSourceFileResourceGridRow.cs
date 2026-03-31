namespace WADNR.Models.DataTransferObjects;

public class FundSourceFileResourceGridRow
{
    public int FundSourceFileResourceID { get; set; }
    public int FileResourceID { get; set; }
    public Guid FileResourceGUID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? OriginalBaseFilename { get; set; }
    public string? FileResourceMimeTypeName { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}
