namespace WADNR.Models.DataTransferObjects;

public class PersonFirstNameLastName
{
    public int PersonID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}