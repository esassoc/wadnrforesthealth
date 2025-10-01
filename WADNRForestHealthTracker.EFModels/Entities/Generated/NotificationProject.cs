using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("NotificationProject")]
[Index("NotificationID", "ProjectID", Name = "AK_NotificationProject_NotificationID_ProjectID", IsUnique = true)]
public partial class NotificationProject
{
    [Key]
    public int NotificationProjectID { get; set; }

    public int NotificationID { get; set; }

    public int ProjectID { get; set; }

    [ForeignKey("NotificationID")]
    [InverseProperty("NotificationProjects")]
    public virtual Notification Notification { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("NotificationProjects")]
    public virtual Project Project { get; set; } = null!;
}
