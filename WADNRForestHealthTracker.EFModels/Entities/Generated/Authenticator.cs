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

    [StringLength(10)]
    [Unicode(false)]
    public string AuthenticatorName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string AuthenticatorFullName { get; set; } = null!;

    [InverseProperty("Authenticator")]
    public virtual ICollection<PersonAllowedAuthenticator> PersonAllowedAuthenticators { get; set; } = new List<PersonAllowedAuthenticator>();
}
