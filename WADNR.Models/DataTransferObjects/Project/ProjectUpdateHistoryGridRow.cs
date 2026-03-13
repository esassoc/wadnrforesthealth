namespace WADNR.Models.DataTransferObjects;

public class ProjectUpdateHistoryGridRow
{
    public int ProjectUpdateBatchID { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; }
    public string LastUpdatePersonName { get; set; } = string.Empty;
    public string ProjectUpdateStateName { get; set; } = string.Empty;
}
