namespace WADNR.Models.DataTransferObjects.Shared;

public class LegacyGeometry
{
    public int CoordinateSystemId { get; set; }
    public string WellKnownText { get; set; }
}

public class LegacyGeometryWrapper
{
    public LegacyGeometry Geometry { get; set; }
}
