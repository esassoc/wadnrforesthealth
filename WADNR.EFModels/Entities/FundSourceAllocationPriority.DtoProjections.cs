using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocationPriorityProjections
{
    public static readonly Expression<Func<FundSourceAllocationPriority, FundSourceAllocationPriorityLookupItem>> AsLookupItem = x => new FundSourceAllocationPriorityLookupItem
    {
        FundSourceAllocationPriorityID = x.FundSourceAllocationPriorityID,
        FundSourceAllocationPriorityNumber = x.FundSourceAllocationPriorityNumber,
        FundSourceAllocationPriorityColor = x.FundSourceAllocationPriorityColor
    };
}
