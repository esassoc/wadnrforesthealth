namespace WADNR.Models.DataTransferObjects;

public class AgreementContactGridRow
{
    public int AgreementPersonID { get; set; }
    public PersonFirstNameLastName Person { get; set; } = new();
    public AgreementPersonRoleLookupItem AgreementRole { get; set; } = new();
    public OrganizationLookupItem? ContributingOrganization { get; set; }
}
