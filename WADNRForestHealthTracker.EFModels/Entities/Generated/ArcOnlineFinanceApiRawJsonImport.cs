using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNRForestHealthTracker.EFModels.Entities;

[Table("ArcOnlineFinanceApiRawJsonImport")]
public partial class ArcOnlineFinanceApiRawJsonImport
{
    [Key]
    public int ArcOnlineFinanceApiRawJsonImportID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int ArcOnlineFinanceApiRawJsonImportTableTypeID { get; set; }

    public int? BienniumFiscalYear { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FinanceApiLastLoadDate { get; set; }

    [Unicode(false)]
    public string RawJsonString { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? JsonImportDate { get; set; }

    public int JsonImportStatusTypeID { get; set; }
}
