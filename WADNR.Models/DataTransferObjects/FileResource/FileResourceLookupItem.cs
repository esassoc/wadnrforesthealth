namespace WADNR.Models.DataTransferObjects;

public class FileResourceLookupItem
{
    public int FileResourceID { get; set; }
    public Guid FileResourceGUID { get; set; }
    public string OriginalBaseFilename { get; set; } = string.Empty;
    public string OriginalFileExtension { get; set; } = string.Empty;

    public string OriginalCompleteFileName => $"{OriginalBaseFilename}.{OriginalFileExtension}";
}
