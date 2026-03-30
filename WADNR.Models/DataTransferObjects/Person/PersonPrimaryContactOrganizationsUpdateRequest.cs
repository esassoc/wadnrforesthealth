namespace WADNR.Models.DataTransferObjects;

public class PersonPrimaryContactOrganizationsUpdateRequest
{
    public List<int> OrganizationIDs { get; set; } = new();
}
