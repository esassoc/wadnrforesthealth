using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects.ProjectUpdate;

public class ProjectUpdateConfigurationUpsertRequest
{
    [Required]
    public bool EnableProjectUpdateReminders { get; set; }

    public DateTime? ProjectUpdateKickOffDate { get; set; }
    public string? ProjectUpdateKickOffIntroContent { get; set; }

    [Required]
    public bool SendPeriodicReminders { get; set; }

    [Range(7, 365)]
    public int? ProjectUpdateReminderInterval { get; set; }
    public string? ProjectUpdateReminderIntroContent { get; set; }

    [Required]
    public bool SendCloseOutNotification { get; set; }

    public DateTime? ProjectUpdateCloseOutDate { get; set; }
    public string? ProjectUpdateCloseOutIntroContent { get; set; }
}
