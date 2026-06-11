namespace WADNR.Models.DataTransferObjects.ServiceForestryUpload;

public class ServiceForestryUploadResult
{
    public int RecordsImported { get; set; }
    public double ElapsedSeconds { get; set; }
    public List<string> Warnings { get; set; } = new();
}
