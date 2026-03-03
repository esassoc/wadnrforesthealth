using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class ProgramNotificationUpsertRequest
{
    [Required]
    public int ProgramNotificationTypeID { get; set; }

    [Required]
    public int RecurrenceIntervalID { get; set; }

    [Required]
    public string NotificationEmailText { get; set; } = string.Empty;
}
