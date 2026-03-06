using System;

namespace WADNR.Models.DataTransferObjects.Job;

public class ImportHistoryDto
{
    public int ArcOnlineFinanceApiRawJsonImportID { get; set; }
    public DateTime CreateDate { get; set; }
    public int ArcOnlineFinanceApiRawJsonImportTableTypeID { get; set; }
    public int? BienniumFiscalYear { get; set; }
    public DateTime? FinanceApiLastLoadDate { get; set; }
    public DateTime? JsonImportDate { get; set; }
    public int JsonImportStatusTypeID { get; set; }
}
