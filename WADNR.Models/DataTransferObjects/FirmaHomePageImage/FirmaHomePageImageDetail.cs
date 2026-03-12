namespace WADNR.Models.DataTransferObjects;

public class FirmaHomePageImageDetail
{
    public int FirmaHomePageImageID { get; set; }
    public Guid FileResourceGUID { get; set; }
    public string Caption { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public long? ContentLength { get; set; }
}
