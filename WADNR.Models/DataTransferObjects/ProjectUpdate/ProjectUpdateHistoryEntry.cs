namespace WADNR.Models.DataTransferObjects.ProjectUpdate;

public class ProjectUpdateHistoryEntry
{
    public DateTimeOffset TransitionDate { get; set; }
    public int ProjectUpdateStateID { get; set; }
    public string? ProjectUpdateStateName { get; set; }
    public string UpdatePersonName { get; set; } = string.Empty;
}
