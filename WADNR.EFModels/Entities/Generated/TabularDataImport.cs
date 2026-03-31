using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Table("TabularDataImport")]
public partial class TabularDataImport
{
    [Key]
    public int TabularDataImportID { get; set; }

    public int TabularDataImportTableTypeID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UploadDate { get; set; }

    public int? UploadPersonID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastProcessedDate { get; set; }

    public int? LastProcessedPersonID { get; set; }

    [ForeignKey("LastProcessedPersonID")]
    [InverseProperty("TabularDataImportLastProcessedPeople")]
    public virtual Person? LastProcessedPerson { get; set; }

    [ForeignKey("UploadPersonID")]
    [InverseProperty("TabularDataImportUploadPeople")]
    public virtual Person? UploadPerson { get; set; }
}
