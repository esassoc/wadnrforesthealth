namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationBudgetLineItemUpsertRequest
{
    public List<FundSourceAllocationBudgetLineItemUpsertItem> Items { get; set; } = new();
}

public class FundSourceAllocationBudgetLineItemUpsertItem
{
    public int CostTypeID { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}
