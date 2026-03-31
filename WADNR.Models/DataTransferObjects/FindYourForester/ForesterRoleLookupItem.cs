namespace WADNR.Models.DataTransferObjects.FindYourForester;

public class ForesterRoleLookupItem
{
    public int ForesterRoleID { get; set; }
    public string ForesterRoleDisplayName { get; set; } = string.Empty;
    public string ForesterRoleName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
