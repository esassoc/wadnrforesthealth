using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("InteractionEventFileResource")]
[Index("FileResourceID", Name = "AK_InteractionEventFileResource_FileResourceID", IsUnique = true)]
public partial class InteractionEventFileResource
{
    [Key]
    public int InteractionEventFileResourceID { get; set; }

    public int InteractionEventID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? Description { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("InteractionEventFileResource")]
    public virtual FileResource FileResource { get; set; } = null!;

    [ForeignKey("InteractionEventID")]
    [InverseProperty("InteractionEventFileResources")]
    public virtual InteractionEvent InteractionEvent { get; set; } = null!;
}
