namespace WADNR.Models.DataTransferObjects;

public class AgreementContactGridRowRaw
{
    public int AgreementPersonID { get; set; }
    public int AgreementPersonRoleID { get; set; }
    public int PersonID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public int? OrganizationID { get; set; }
    public string? OrganizationName { get; set; }
}
