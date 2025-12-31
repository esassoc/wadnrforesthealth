using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("ProjectPersonUpdate")]
public partial class ProjectPersonUpdate
{
    [Key]
    public int ProjectPersonUpdateID { get; set; }

    public int ProjectUpdateBatchID { get; set; }

    public int PersonID { get; set; }

    public int ProjectPersonRelationshipTypeID { get; set; }

    [ForeignKey("PersonID")]
    [InverseProperty("ProjectPersonUpdates")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("ProjectUpdateBatchID")]
    [InverseProperty("ProjectPersonUpdates")]
    public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; } = null!;
}
