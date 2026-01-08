using System;
using System.Linq;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocationProjections
{
    public static readonly Expression<Func<FundSourceAllocation, FundSourceAllocationDNRUplandRegionDetailGridRow>> AsDnrUplandRegionDetailGridRow = x => new FundSourceAllocationDNRUplandRegionDetailGridRow
    {
        FundSourceAllocationID = x.FundSourceAllocationID,
        FundSourceAllocationName = x.FundSourceAllocationName,
        FundSourceEndDate = x.EndDate,
        HasFundFSPs = x.HasFundFSPs,
        FundSourceAllocationPriority = new FundSourceAllocationPriorityDetail
        {
            FundSourceAllocationPriorityID = x.FundSourceAllocationPriorityID,
            FundSourceAllocationPriorityName = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityNumber.ToString() : null,
            FundSourceAllocationPriorityColor = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityColor : null
        },
        ProgramIndices = x.FundSourceAllocationProgramIndexProjectCodes
            .Where(y => y.ProgramIndex != null)
            .Select(y => new ProgramIndexLookupItem
            {
                ProgramIndexID = y.ProgramIndex.ProgramIndexID,
                ProgramIndexCode = y.ProgramIndex.ProgramIndexCode
            })
            .ToList(),
        ProjectCodes = x.FundSourceAllocationProgramIndexProjectCodes
            .Where(y => y.ProjectCode != null)
            .Select(y => new ProjectCodeLookupItem
            {
                ProjectCodeID = y.ProjectCode!.ProjectCodeID,
                ProjectCodeName = y.ProjectCode!.ProjectCodeName
            })
            .Distinct()
            .ToList(),
        FundSource = new FundSourceLookupItem
        {
            FundSourceID = x.FundSource.FundSourceID,
            FundSourceNumber = x.FundSource.FundSourceNumber
        },
        FundSourceAllocationSource = x.FundSourceAllocationSource != null
            ? new FundSourceAllocationSourceLookupItem
            {
                FundSourceAllocationSourceID = x.FundSourceAllocationSource.FundSourceAllocationSourceID,
                FundSourceAllocationSourceDisplayName = x.FundSourceAllocationSource.FundSourceAllocationSourceDisplayName
            }
            : null,
        AllocationAmount = x.AllocationAmount,
        AllocationPercentage =
            (
                (x.FundSourceAllocationBudgetLineItems
                    .Where(b => b.CostTypeID == (int)CostTypeEnum.Contractual)
                    .Select(b => (decimal?)b.FundSourceAllocationBudgetLineItemAmount)
                    .Sum() ?? 0m)
            ) == 0m
            ? (decimal?)null
            : Math.Round(
                (
                    (x.ProjectFundSourceAllocationRequests.Select(r => (decimal?)r.PayAmount).Sum() ?? 0m)
                    /
                    (x.FundSourceAllocationBudgetLineItems
                        .Where(b => b.CostTypeID == (int)CostTypeEnum.Contractual)
                        .Select(b => (decimal?)b.FundSourceAllocationBudgetLineItemAmount)
                        .Sum() ?? 0m)
                ) * 100m, 2),
        LikelyToUsePeople = x.LikelyToUse == true
            ? x.FundSourceAllocationLikelyPeople
                .Select(lp => new PersonLookupItem
                {
                    PersonID = lp.Person.PersonID,
                    FullName = lp.Person.FirstName + " " + lp.Person.LastName
                })
                .ToList()
            : new List<PersonLookupItem>
                {
                    new()
                    {
                        PersonID = 0,
                        FullName = x.LikelyToUse == false ? "Contractual Only" : "N/A"
                    }
                }
    };
}
