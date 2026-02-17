namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationBudgetLineItemGridRow
{
    public int FundSourceAllocationBudgetLineItemID { get; set; }
    public int CostTypeID { get; set; }
    public string? CostTypeName { get; set; }
    public decimal FundSourceAllocationBudgetLineItemAmount { get; set; }
    public string? FundSourceAllocationBudgetLineItemNote { get; set; }
}
