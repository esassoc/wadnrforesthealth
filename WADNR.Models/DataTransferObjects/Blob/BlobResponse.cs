namespace WADNR.Models.DataTransferObjects;

public class BlobResponse
{
    public string Status { get; set; }
    public bool Error { get; set; }
    public BlobFile Blob { get; set; } = new();
}