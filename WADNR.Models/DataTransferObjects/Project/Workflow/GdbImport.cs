namespace WADNR.Models.DataTransferObjects;

public class GdbFeatureClassPreview
{
    public string FeatureClassName { get; set; } = string.Empty;
    public string FeatureType { get; set; } = string.Empty;
    public int FeatureCount { get; set; }
    public List<string> PropertyNames { get; set; } = new();
}

public class GdbApproveRequest
{
    public List<GdbLayerApproval> Layers { get; set; } = new();
}

public class GdbLayerApproval
{
    public string FeatureClassName { get; set; } = string.Empty;
    public string SelectedPropertyName { get; set; } = string.Empty;
    public bool ShouldImport { get; set; }
}
