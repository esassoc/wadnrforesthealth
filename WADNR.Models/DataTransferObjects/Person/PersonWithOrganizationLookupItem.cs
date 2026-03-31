namespace WADNR.Models.DataTransferObjects;

public class PersonWithOrganizationLookupItem
{
    public int PersonID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? OrganizationShortName { get; set; }
}
