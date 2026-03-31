namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationNoteInternalDetail
{
    public int FundSourceAllocationNoteInternalID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public string? Note { get; set; }
    public string? CreatedByPersonName { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public string? UpdatedByPersonName { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
}
