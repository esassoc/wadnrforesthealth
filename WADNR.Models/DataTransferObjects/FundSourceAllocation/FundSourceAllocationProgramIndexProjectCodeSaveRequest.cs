namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationProgramIndexProjectCodeSaveRequest
{
    public List<FundSourceAllocationProgramIndexProjectCodePair> Pairs { get; set; } = new();
}

public class FundSourceAllocationProgramIndexProjectCodePair
{
    public int ProgramIndexID { get; set; }
    public int? ProjectCodeID { get; set; }
}
