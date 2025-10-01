using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProgramNotificationSent")]
public partial class ProgramNotificationSent
{
    [Key]
    public int ProgramNotificationSentID { get; set; }

    public int ProgramNotificationConfigurationID { get; set; }

    public int SentToPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ProgramNotificationSentDate { get; set; }

    [ForeignKey("ProgramNotificationConfigurationID")]
    [InverseProperty("ProgramNotificationSents")]
    public virtual ProgramNotificationConfiguration ProgramNotificationConfiguration { get; set; } = null!;

    [InverseProperty("ProgramNotificationSent")]
    public virtual ICollection<ProgramNotificationSentProject> ProgramNotificationSentProjects { get; set; } = new List<ProgramNotificationSentProject>();

    [ForeignKey("SentToPersonID")]
    [InverseProperty("ProgramNotificationSents")]
    public virtual Person SentToPerson { get; set; } = null!;
}
