using NetTopologySuite.Features;

namespace WADNR.Models.DataTransferObjects;

public class GenericLayer
{
    public string LayerName { get; set; } = string.Empty;

    public string LayerColor { get; set; } = string.Empty;

    public FeatureCollection Features { get; set; } = new();
}
