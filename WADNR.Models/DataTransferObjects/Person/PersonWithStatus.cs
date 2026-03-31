namespace WADNR.Models.DataTransferObjects;

public class PersonWithStatus
{
    public int PersonID { get; set; }
    public string FullNameFirstLast { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
