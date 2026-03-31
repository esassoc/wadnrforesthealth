namespace WADNR.Models.DataTransferObjects;

public class RoleGridRow
{
    public int RoleID { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleDisplayName { get; set; } = string.Empty;
    public string? RoleDescription { get; set; }
    public bool IsBaseRole { get; set; }
    public int PeopleCount { get; set; }
}
