namespace WADNR.Models.DataTransferObjects.FocusArea;

public class FocusAreaGridRow
{
    public int FocusAreaID { get; set; }
    public string FocusAreaName { get; set; } = string.Empty;
    public int FocusAreaStatusID { get; set; }
    public string FocusAreaStatusDisplayName { get; set; } = string.Empty;
    public int DNRUplandRegionID { get; set; }
    public string DNRUplandRegionName { get; set; } = string.Empty;
    public decimal? PlannedFootprintAcres { get; set; }
    public int ProjectCount { get; set; }
    public bool HasLocation { get; set; }
}
