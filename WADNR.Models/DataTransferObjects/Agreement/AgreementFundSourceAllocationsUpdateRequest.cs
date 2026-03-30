namespace WADNR.Models.DataTransferObjects;

public class AgreementFundSourceAllocationsUpdateRequest
{
    public List<int> FundSourceAllocationIDs { get; set; } = new();
}
