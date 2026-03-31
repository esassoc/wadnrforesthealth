namespace WADNR.Models.DataTransferObjects;

public class AgreementLookupItem
{
    public int AgreementID { get; set; }
    public string AgreementTitle { get; set; } = string.Empty;
    public string? AgreementNumber { get; set; }
}
