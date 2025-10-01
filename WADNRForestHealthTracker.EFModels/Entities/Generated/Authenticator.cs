using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("Authenticator")]
public partial class Authenticator
{
    [Key]
    public int AuthenticatorID { get; set; }

    [InverseProperty("Authenticator")]
    public virtual ICollection<PersonAllowedAuthenticator> PersonAllowedAuthenticators { get; set; } = new List<PersonAllowedAuthenticator>();
}
