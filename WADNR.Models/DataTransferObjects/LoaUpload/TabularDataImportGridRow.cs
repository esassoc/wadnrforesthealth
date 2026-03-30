namespace WADNR.Models.DataTransferObjects.LoaUpload;

public class TabularDataImportGridRow
{
    public int TabularDataImportID { get; set; }
    public int TabularDataImportTableTypeID { get; set; }
    public DateTimeOffset? UploadDate { get; set; }
    public string? UploadPersonName { get; set; }
    public DateTimeOffset? LastProcessedDate { get; set; }
    public string? LastProcessedPersonName { get; set; }
}
