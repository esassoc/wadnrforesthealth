namespace WADNR.Models.DataTransferObjects;

public class PersonApiKey
{
    public string? ApiKey { get; set; }
    public DateTimeOffset? ApiKeyGeneratedDate { get; set; }
}
