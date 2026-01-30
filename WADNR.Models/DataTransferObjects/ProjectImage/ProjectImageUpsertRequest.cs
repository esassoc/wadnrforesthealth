namespace WADNR.Models.DataTransferObjects;

public class ProjectImageUpsertRequest
{
    public string Caption { get; set; } = string.Empty;
    public string Credit { get; set; } = string.Empty;
    public int? ProjectImageTimingID { get; set; }
    public bool ExcludeFromFactSheet { get; set; }
}
