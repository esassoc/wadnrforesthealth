namespace WADNR.Models.DataTransferObjects.LoaUpload;

public class LoaPublishingResult
{
    public bool Success { get; set; }
    public double ElapsedSeconds { get; set; }
    public string? ErrorMessage { get; set; }
}
