namespace WADNR.Models.DataTransferObjects;

public class CountyGridRow
{
    public int CountyID { get; set; }
    public string CountyName { get; set; } = string.Empty;
    public int ProjectCount { get; set; }
}
