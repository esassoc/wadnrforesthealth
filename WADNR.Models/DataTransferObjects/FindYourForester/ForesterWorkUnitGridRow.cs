namespace WADNR.Models.DataTransferObjects.FindYourForester;

public class ForesterWorkUnitGridRow
{
    public int ForesterWorkUnitID { get; set; }
    public int ForesterRoleID { get; set; }
    public string ForesterRoleDisplayName { get; set; } = string.Empty;
    public string ForesterWorkUnitName { get; set; } = string.Empty;
    public string? RegionName { get; set; }
    public int? PersonID { get; set; }
    public string? AssignedPersonName { get; set; }
}
