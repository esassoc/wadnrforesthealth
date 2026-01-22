namespace WADNR.Models.DataTransferObjects;

public class PersonGridRow
{
    public int PersonID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public int? OrganizationID { get; set; }
    public string? OrganizationName { get; set; }
    public string? OrganizationShortName { get; set; }
    public string? Phone { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public string? RoleName { get; set; }
    public string? SupplementalRoles { get; set; }
    public bool IsActive { get; set; }
    public int PrimaryContactOrganizationCount { get; set; }
    public DateTime CreateDate { get; set; }
    public int? AddedByPersonID { get; set; }
    public string? AddedByPersonName { get; set; }
}
