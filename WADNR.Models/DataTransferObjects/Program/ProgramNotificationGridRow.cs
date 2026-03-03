namespace WADNR.Models.DataTransferObjects;

public class ProgramNotificationGridRow
{
    public int ProgramNotificationConfigurationID { get; set; }
    public int ProgramNotificationTypeID { get; set; }
    public string ProgramNotificationTypeDisplayName { get; set; } = string.Empty;
    public int RecurrenceIntervalID { get; set; }
    public int RecurrenceIntervalInYears { get; set; }
    public string? NotificationEmailText { get; set; }
}
