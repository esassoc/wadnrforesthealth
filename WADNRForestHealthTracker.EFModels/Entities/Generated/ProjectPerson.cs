using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ProjectPerson")]
public partial class ProjectPerson
{
    [Key]
    public int ProjectPersonID { get; set; }

    public int ProjectID { get; set; }

    public int PersonID { get; set; }

    public int ProjectPersonRelationshipTypeID { get; set; }

    public bool? CreatedAsPartOfBulkImport { get; set; }

    [ForeignKey("PersonID")]
    [InverseProperty("ProjectPeople")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectPerson")]
    public virtual Project Project { get; set; } = null!;
}
