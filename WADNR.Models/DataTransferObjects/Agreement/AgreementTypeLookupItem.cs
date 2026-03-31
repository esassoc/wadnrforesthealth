namespace WADNR.Models.DataTransferObjects;

public class AgreementTypeLookupItem
{
    public int AgreementTypeID { get; set; }
    public string AgreementTypeName { get; set; } = string.Empty;
    public string AgreementTypeAbbrev { get; set; } = string.Empty;
}
