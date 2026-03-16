namespace WADNR.Models.DataTransferObjects;

public class PersonRolesUpsertRequest
{
    public int BaseRoleID { get; set; }
    public List<int> SupplementalRoleIDs { get; set; } = new();
    public bool ReceiveSupportEmails { get; set; }
}
