namespace WADNR.Models.DataTransferObjects;

public class FundSourceFileUpdateRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
