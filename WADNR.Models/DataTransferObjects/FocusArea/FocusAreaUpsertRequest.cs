namespace WADNR.Models.DataTransferObjects.FocusArea;

public class FocusAreaUpsertRequest
{
    public string FocusAreaName { get; set; } = string.Empty;
    public int FocusAreaStatusID { get; set; }
    public int DNRUplandRegionID { get; set; }
    public decimal? PlannedFootprintAcres { get; set; }
}
