namespace WADNR.Models.DataTransferObjects.GisBulkImport;

public class GisMetadataAttributeItem
{
    public int GisMetadataAttributeID { get; set; }
    public string GisMetadataAttributeName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
