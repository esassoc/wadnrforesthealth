namespace WADNR.Models.DataTransferObjects.FindYourForester;

public class FindYourForesterPointResult
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<ForesterContactResult> ForesterContacts { get; set; } = new();
}

public class ForesterContactResult
{
    public int ForesterWorkUnitID { get; set; }
    public int ForesterRoleID { get; set; }
    public string ForesterRoleDisplayName { get; set; }
    public int? PersonID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ForesterWorkUnitName { get; set; }
    public string ForesterRoleDefinition { get; set; }
}
