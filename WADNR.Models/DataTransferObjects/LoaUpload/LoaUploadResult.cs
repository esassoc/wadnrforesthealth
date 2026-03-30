namespace WADNR.Models.DataTransferObjects.LoaUpload;

public class LoaUploadResult
{
    public int RecordsImported { get; set; }
    public double ElapsedSeconds { get; set; }
    public List<string> Warnings { get; set; } = new();
}
