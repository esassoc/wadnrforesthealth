using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("Notification")]
public partial class Notification
{
    [Key]
    public int NotificationID { get; set; }

    public int NotificationTypeID { get; set; }

    public int PersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NotificationDate { get; set; }

    [InverseProperty("Notification")]
    public virtual ICollection<NotificationProject> NotificationProjects { get; set; } = new List<NotificationProject>();

    [ForeignKey("PersonID")]
    [InverseProperty("Notifications")]
    public virtual Person Person { get; set; } = null!;
}
