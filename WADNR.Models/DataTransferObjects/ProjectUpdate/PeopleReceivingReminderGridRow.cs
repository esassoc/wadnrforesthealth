namespace WADNR.Models.DataTransferObjects.ProjectUpdate;

public class PeopleReceivingReminderGridRow
{
    public int PersonID { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public int? OrganizationID { get; set; }
    public string? OrganizationName { get; set; }
    public string? OrganizationShortName { get; set; }
    public string? Email { get; set; }
    public int ProjectsRequiringUpdate { get; set; }
    public int UpdatesNotStarted { get; set; }
    public int UpdatesInProgress { get; set; }
    public int UpdatesSubmitted { get; set; }
    public int UpdatesReturned { get; set; }
    public int UpdatesApproved { get; set; }
    public int RemindersSent { get; set; }
    public DateTime? LastReminderDate { get; set; }
}
