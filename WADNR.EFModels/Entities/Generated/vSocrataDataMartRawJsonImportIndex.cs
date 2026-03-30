using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

[Keyless]
public partial class vSocrataDataMartRawJsonImportIndex
{
    public int SocrataDataMartRawJsonImportID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int SocrataDataMartRawJsonImportTableTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string SocrataDataMartRawJsonImportTableTypeName { get; set; } = null!;

    public int? BienniumFiscalYear { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FinanceApiLastLoadDate { get; set; }

    public int JsonImportStatusTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string JsonImportStatusTypeName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? JsonImportDate { get; set; }

    public long? RawJsonStringLength { get; set; }
}
