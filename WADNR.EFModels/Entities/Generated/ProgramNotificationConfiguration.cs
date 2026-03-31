using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProgramNotificationConfiguration")]
public partial class ProgramNotificationConfiguration
{
    [Key]
    public int ProgramNotificationConfigurationID { get; set; }

    public int ProgramID { get; set; }

    public int ProgramNotificationTypeID { get; set; }

    public int RecurrenceIntervalID { get; set; }

    [Unicode(false)]
    public string NotificationEmailText { get; set; } = null!;

    [ForeignKey("ProgramID")]
    [InverseProperty("ProgramNotificationConfigurations")]
    public virtual Program Program { get; set; } = null!;

    [InverseProperty("ProgramNotificationConfiguration")]
    public virtual ICollection<ProgramNotificationSent> ProgramNotificationSents { get; set; } = new List<ProgramNotificationSent>();
}
