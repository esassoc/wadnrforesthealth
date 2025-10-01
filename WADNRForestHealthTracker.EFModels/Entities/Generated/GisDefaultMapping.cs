using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("GisDefaultMapping")]
public partial class GisDefaultMapping
{
    [Key]
    public int GisDefaultMappingID { get; set; }

    public int GisUploadSourceOrganizationID { get; set; }

    public int FieldDefinitionID { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string GisDefaultMappingColumnName { get; set; } = null!;

    [ForeignKey("GisUploadSourceOrganizationID")]
    [InverseProperty("GisDefaultMappings")]
    public virtual GisUploadSourceOrganization GisUploadSourceOrganization { get; set; } = null!;
}
