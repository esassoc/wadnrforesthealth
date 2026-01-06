namespace WADNR.Models.DataTransferObjects;

public class DNRUplandRegionUpsertRequest
{
    public string DNRUplandRegionName { get; set; } = string.Empty;
    public string? DNRUplandRegionAbbrev { get; set; }
    public string? RegionAddress { get; set; }
    public string? RegionCity { get; set; }
    public string? RegionState { get; set; }
    public string? RegionZip { get; set; }
    public string? RegionPhone { get; set; }
    public string? RegionEmail { get; set; }
}
