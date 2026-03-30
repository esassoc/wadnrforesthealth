namespace WADNR.Models.DataTransferObjects.FocusArea;

public class FocusAreaCloseoutProjectItem
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int ProjectStageID { get; set; }
    public string ProjectStageDisplayName { get; set; } = string.Empty;
    public decimal? EstimatedTotalCost { get; set; }
}
