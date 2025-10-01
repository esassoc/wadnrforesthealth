using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("PersonAllowedAuthenticator")]
public partial class PersonAllowedAuthenticator
{
    [Key]
    public int PersonAllowedAuthenticatorID { get; set; }

    public int PersonID { get; set; }

    public int AuthenticatorID { get; set; }

    [ForeignKey("AuthenticatorID")]
    [InverseProperty("PersonAllowedAuthenticators")]
    public virtual Authenticator Authenticator { get; set; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("PersonAllowedAuthenticators")]
    public virtual Person Person { get; set; } = null!;
}
