namespace WADNR.Models.DataTransferObjects;

public class AgreementContactGridRow
{
    public PersonFirstNameLastName Person { get; set; } = new();
    public AgreementRoleLookupItem AgreementRole { get; set; } = new();
    public OrganizationLookupItem? ContributingOrganization { get; set; }
}
