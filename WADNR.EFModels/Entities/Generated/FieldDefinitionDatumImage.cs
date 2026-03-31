using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("FieldDefinitionDatumImage")]
public partial class FieldDefinitionDatumImage
{
    [Key]
    public int FieldDefinitionDatumImageID { get; set; }

    public int FieldDefinitionDatumID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("FieldDefinitionDatumID")]
    [InverseProperty("FieldDefinitionDatumImages")]
    public virtual FieldDefinitionDatum FieldDefinitionDatum { get; set; } = null!;

    [ForeignKey("FileResourceID")]
    [InverseProperty("FieldDefinitionDatumImages")]
    public virtual FileResource FileResource { get; set; } = null!;
}
