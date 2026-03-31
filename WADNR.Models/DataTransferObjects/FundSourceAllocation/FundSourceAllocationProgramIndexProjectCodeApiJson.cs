namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationProgramIndexProjectCodeApiJson
{
    public int FundSourceAllocationProgramIndexProjectCodeID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public int ProgramIndexID { get; set; }
    public string ProgramIndexCode { get; set; }
    public int? ProjectCodeID { get; set; }
    public string ProjectCodeName { get; set; }
}
