namespace WADNR.Models.DataTransferObjects.GisBulkImport;

public class GisFeatureGridRow
{
    public int GisFeatureID { get; set; }
    public int GisImportFeatureKey { get; set; }
    public bool? IsValid { get; set; }
    public decimal? CalculatedArea { get; set; }
    public Dictionary<string, string?> MetadataValues { get; set; } = new();
}
