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
        FundSourceAllocationPriorityDetail = new FundSourceAllocationPriorityDetail
        {
            FundSourceAllocationPriorityID = x.FundSourceAllocationPriorityID,
            FundSourceAllocationPriorityName = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityNumber.ToString() : null,
            FundSourceAllocationPriorityColor = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityColor : null
        },
        ProgramIndexLookupItems = x.FundSourceAllocationProgramIndexProjectCodes
            .Where(y => y.ProgramIndex != null)
            .Select(y => new ProgramIndexLookupItem
            {
                ProgramIndexID = y.ProgramIndex.ProgramIndexID,
                ProgramIndexCode = y.ProgramIndex.ProgramIndexCode
            })
            .ToList(),
        ProjectCodeLookupItems = x.FundSourceAllocationProgramIndexProjectCodes
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
        ExpectedFundingByProject = x.ProjectFundSourceAllocationRequests.Sum(y => (decimal?)y.PayAmount) ?? 0m,
        AllocationAmount = x.AllocationAmount,
        BudgetLineItem = x.FundSourceAllocationBudgetLineItems
            .Where(bli => bli.CostTypeID == CostType.Contractual.CostTypeID)
            .Select(bli => (decimal?)bli.FundSourceAllocationBudgetLineItemAmount)
            .SingleOrDefault(),
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
