namespace WADNR.Models.DataTransferObjects;

public class ProgramIndexLookupItem
{
    public int ProgramIndexID { get; set; }
    public string ProgramIndexCode { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
