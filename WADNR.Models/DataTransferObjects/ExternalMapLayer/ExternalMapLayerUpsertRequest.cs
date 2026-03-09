namespace WADNR.Models.DataTransferObjects;

public class ExternalMapLayerUpsertRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string LayerUrl { get; set; } = string.Empty;
    public string? LayerDescription { get; set; }
    public string? FeatureNameField { get; set; }
    public bool DisplayOnProjectMap { get; set; }
    public bool DisplayOnPriorityLandscape { get; set; }
    public bool DisplayOnAllOthers { get; set; }
    public bool IsActive { get; set; }
    public bool IsTiledMapService { get; set; }
}
