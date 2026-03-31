namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationProgramIndexProjectCodeItem
{
    public int FundSourceAllocationProgramIndexProjectCodeID { get; set; }
    public int ProgramIndexID { get; set; }
    public string ProgramIndexCode { get; set; } = string.Empty;
    public int? ProjectCodeID { get; set; }
    public string? ProjectCodeName { get; set; }
}
