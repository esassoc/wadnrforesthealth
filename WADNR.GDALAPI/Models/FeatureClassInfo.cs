namespace WADNR.GDALAPI.Models;

public class FeatureClassInfo
{
    public string LayerName { get; set; } = string.Empty;
    public string FeatureType { get; set; } = string.Empty;
    public int FeatureCount { get; set; }
    public List<string> Columns { get; set; } = new();
}
