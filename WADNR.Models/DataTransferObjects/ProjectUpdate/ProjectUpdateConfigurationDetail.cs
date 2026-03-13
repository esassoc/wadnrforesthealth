namespace WADNR.Models.DataTransferObjects.ProjectUpdate;

public class ProjectUpdateConfigurationDetail
{
    public int ProjectUpdateConfigurationID { get; set; }
    public bool EnableProjectUpdateReminders { get; set; }
    public DateOnly? ProjectUpdateKickOffDate { get; set; }
    public string? ProjectUpdateKickOffIntroContent { get; set; }
    public bool SendPeriodicReminders { get; set; }
    public int? ProjectUpdateReminderInterval { get; set; }
    public string? ProjectUpdateReminderIntroContent { get; set; }
    public bool SendCloseOutNotification { get; set; }
    public DateOnly? ProjectUpdateCloseOutDate { get; set; }
    public string? ProjectUpdateCloseOutIntroContent { get; set; }
}
