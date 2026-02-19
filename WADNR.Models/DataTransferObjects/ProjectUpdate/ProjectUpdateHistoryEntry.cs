namespace WADNR.Models.DataTransferObjects.ProjectUpdate;

public class ProjectUpdateHistoryEntry
{
    public DateTime TransitionDate { get; set; }
    public int ProjectUpdateStateID { get; set; }
    public string? ProjectUpdateStateName { get; set; }
    public string UpdatePersonName { get; set; } = string.Empty;
}
