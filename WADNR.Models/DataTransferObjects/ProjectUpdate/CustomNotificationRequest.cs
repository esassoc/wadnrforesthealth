using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects.ProjectUpdate;

public class CustomNotificationRequest
{
    [Required]
    public List<int> PersonIDList { get; set; } = new();

    [Required]
    public string Subject { get; set; }

    public string NotificationContent { get; set; }
}
