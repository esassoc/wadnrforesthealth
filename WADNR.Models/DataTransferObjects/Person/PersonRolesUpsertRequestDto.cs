namespace WADNR.Models.DataTransferObjects;

public class PersonRolesUpsertRequestDto
{
    public int BaseRoleID { get; set; }
    public List<int> SupplementalRoleIDs { get; set; } = new();
    public bool ReceiveSupportEmails { get; set; }
}
