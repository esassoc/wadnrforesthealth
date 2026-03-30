namespace WADNR.Models.DataTransferObjects;

public class OrganizationUpsertRequest
{
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationShortName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? OrganizationUrl { get; set; }
    public int? PrimaryContactPersonID { get; set; }
    public int OrganizationTypeID { get; set; }
    public int? VendorID { get; set; }
}
