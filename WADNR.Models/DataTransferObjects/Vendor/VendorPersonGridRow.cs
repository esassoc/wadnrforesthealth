namespace WADNR.Models.DataTransferObjects;

public class VendorPersonGridRow
{
    public int PersonID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
}
