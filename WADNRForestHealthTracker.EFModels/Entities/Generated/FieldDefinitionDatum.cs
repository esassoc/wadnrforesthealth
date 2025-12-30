using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("FieldDefinitionDatum")]
public partial class FieldDefinitionDatum
{
    [Key]
    public int FieldDefinitionDatumID { get; set; }

    public int FieldDefinitionID { get; set; }

    [Unicode(false)]
    public string? FieldDefinitionDatumValue { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? FieldDefinitionLabel { get; set; }

    [InverseProperty("FieldDefinitionDatum")]
    public virtual ICollection<FieldDefinitionDatumImage> FieldDefinitionDatumImages { get; set; } = new List<FieldDefinitionDatumImage>();
}
