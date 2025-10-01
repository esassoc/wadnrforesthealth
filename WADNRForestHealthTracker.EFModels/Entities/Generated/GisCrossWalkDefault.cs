using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("GisCrossWalkDefault")]
public partial class GisCrossWalkDefault
{
    [Key]
    public int GisCrossWalkDefaultID { get; set; }

    public int GisUploadSourceOrganizationID { get; set; }

    public int FieldDefinitionID { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string GisCrossWalkSourceValue { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string GisCrossWalkMappedValue { get; set; } = null!;

    [ForeignKey("GisUploadSourceOrganizationID")]
    [InverseProperty("GisCrossWalkDefaults")]
    public virtual GisUploadSourceOrganization GisUploadSourceOrganization { get; set; } = null!;
}
