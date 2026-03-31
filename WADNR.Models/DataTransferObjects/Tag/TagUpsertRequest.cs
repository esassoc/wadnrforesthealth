namespace WADNR.Models.DataTransferObjects;

public class TagUpsertRequest
{
    public string TagName { get; set; } = string.Empty;
    public string? TagDescription { get; set; }
}
