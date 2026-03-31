namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationNoteUpsertRequest
{
    public int FundSourceAllocationID { get; set; }
    public string Note { get; set; } = string.Empty;
}
