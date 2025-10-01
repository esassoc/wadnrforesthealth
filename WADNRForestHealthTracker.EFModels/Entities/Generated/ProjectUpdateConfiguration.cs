using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectUpdateConfiguration")]
public partial class ProjectUpdateConfiguration
{
    [Key]
    public int ProjectUpdateConfigurationID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ProjectUpdateKickOffDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ProjectUpdateCloseOutDate { get; set; }

    public int? ProjectUpdateReminderInterval { get; set; }

    public bool EnableProjectUpdateReminders { get; set; }

    public bool SendPeriodicReminders { get; set; }

    public bool SendCloseOutNotification { get; set; }

    [Unicode(false)]
    public string? ProjectUpdateKickOffIntroContent { get; set; }

    [Unicode(false)]
    public string? ProjectUpdateReminderIntroContent { get; set; }

    [Unicode(false)]
    public string? ProjectUpdateCloseOutIntroContent { get; set; }
}
