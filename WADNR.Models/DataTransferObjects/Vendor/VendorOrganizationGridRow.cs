namespace WADNR.Models.DataTransferObjects;

public class VendorOrganizationGridRow
{
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? OrganizationShortName { get; set; }
    public bool IsActive { get; set; }
}
