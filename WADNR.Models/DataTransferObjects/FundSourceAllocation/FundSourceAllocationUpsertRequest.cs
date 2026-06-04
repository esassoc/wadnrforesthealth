using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationUpsertRequest
{
    public string? FundSourceAllocationName { get; set; }
    public int FundSourceID { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Allocation Amount must be greater than 0.")]
    public decimal? AllocationAmount { get; set; }
    public int? FederalFundCodeID { get; set; }
    public int? OrganizationID { get; set; }
    public int? DNRUplandRegionID { get; set; }
    public int? DivisionID { get; set; }
    public int? FundSourceManagerID { get; set; }
    public int? FundSourceAllocationPriorityID { get; set; }
    public int? FundSourceAllocationSourceID { get; set; }
    public bool? HasFundFSPs { get; set; }
    public bool? LikelyToUse { get; set; }
    public List<int> ProgramManagerPersonIDs { get; set; } = new();
    public List<int> LikelyToUsePersonIDs { get; set; } = new();
    public string? AllocationAmountChangeNote { get; set; }
}
