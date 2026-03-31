namespace WADNR.Models.DataTransferObjects;

public class FundSourceBudgetLineItemGridRow
{
    public int FundSourceAllocationID { get; set; }
    public string? FundSourceAllocationName { get; set; }
    public decimal PersonnelAmount { get; set; }
    public decimal BenefitsAmount { get; set; }
    public decimal TravelAmount { get; set; }
    public decimal SuppliesAmount { get; set; }
    public decimal ContractualAmount { get; set; }
    public decimal IndirectCostsAmount { get; set; }
    public decimal TotalAmount { get; set; }
}
