using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("InteractionEventContact")]
[Index("InteractionEventID", "PersonID", Name = "AK_InteractionEventContact_InteractionEventID_PersonID", IsUnique = true)]
public partial class InteractionEventContact
{
    [Key]
    public int InteractionEventContactID { get; set; }

    public int InteractionEventID { get; set; }

    public int PersonID { get; set; }

    [ForeignKey("InteractionEventID")]
    [InverseProperty("InteractionEventContacts")]
    public virtual InteractionEvent InteractionEvent { get; set; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("InteractionEventContacts")]
    public virtual Person Person { get; set; } = null!;
}
