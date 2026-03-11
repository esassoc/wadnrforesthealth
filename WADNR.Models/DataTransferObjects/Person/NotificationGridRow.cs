namespace WADNR.Models.DataTransferObjects;

public class NotificationGridRow
{
    public int NotificationID { get; set; }
    public DateTime NotificationDate { get; set; }
    public string NotificationTypeDisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProjectCount { get; set; }
    public int? ProjectID { get; set; }
    public string? ProjectName { get; set; }
}
