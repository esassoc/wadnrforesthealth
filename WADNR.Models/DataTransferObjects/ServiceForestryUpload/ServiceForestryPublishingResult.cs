namespace WADNR.Models.DataTransferObjects.ServiceForestryUpload;

public class ServiceForestryPublishingResult
{
    public bool Success { get; set; }
    public double ElapsedSeconds { get; set; }
    public string? ErrorMessage { get; set; }
}
