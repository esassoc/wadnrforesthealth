namespace WADNR.Models.DataTransferObjects;

public class ProjectNotificationGridRow
{
    public int NotificationID { get; set; }
    public DateTimeOffset NotificationDate { get; set; }
    public string NotificationTypeName { get; set; } = string.Empty;
    public string PersonName { get; set; } = string.Empty;
}
