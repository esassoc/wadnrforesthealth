using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("InteractionEvent")]
[Index("InteractionEventLocationSimple", Name = "SPATIAL_InteractionEvent_InteractionEventLocationSimple")]
public partial class InteractionEvent
{
    [Key]
    public int InteractionEventID { get; set; }

    public int InteractionEventTypeID { get; set; }

    public int? StaffPersonID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string InteractionEventTitle { get; set; } = null!;

    [StringLength(3000)]
    [Unicode(false)]
    public string? InteractionEventDescription { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime InteractionEventDate { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? InteractionEventLocationSimple { get; set; }

    [InverseProperty("InteractionEvent")]
    public virtual ICollection<InteractionEventContact> InteractionEventContacts { get; set; } = new List<InteractionEventContact>();

    [InverseProperty("InteractionEvent")]
    public virtual ICollection<InteractionEventFileResource> InteractionEventFileResources { get; set; } = new List<InteractionEventFileResource>();

    [InverseProperty("InteractionEvent")]
    public virtual ICollection<InteractionEventProject> InteractionEventProjects { get; set; } = new List<InteractionEventProject>();

    [ForeignKey("StaffPersonID")]
    [InverseProperty("InteractionEvents")]
    public virtual Person? StaffPerson { get; set; }
}
