using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProgramPerson")]
[Index("ProgramID", "PersonID", Name = "AK_ProgramPerson_ProgramID_PersonID", IsUnique = true)]
public partial class ProgramPerson
{
    [Key]
    public int ProgramPersonID { get; set; }

    public int ProgramID { get; set; }

    public int PersonID { get; set; }

    [ForeignKey("PersonID")]
    [InverseProperty("ProgramPeople")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("ProgramID")]
    [InverseProperty("ProgramPeople")]
    public virtual Program Program { get; set; } = null!;
}
