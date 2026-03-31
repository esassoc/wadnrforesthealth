namespace WADNR.Models.DataTransferObjects;

public class PersonLookupItem
{
    public int PersonID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
}