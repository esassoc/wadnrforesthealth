using System;
using System.Linq;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

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

    public static readonly Expression<Func<FundSourceAllocation, FundSourceAllocationLookupItem>> AsLookupItem = x => new FundSourceAllocationLookupItem
    {
        FundSourceAllocationID = x.FundSourceAllocationID,
        FundSourceAllocationName = x.FundSource.FundSourceNumber + " " + x.FundSourceAllocationName
    };

    public static readonly Expression<Func<FundSourceAllocation, FundSourceAllocationGridRow>> AsGridRow = x => new FundSourceAllocationGridRow
    {
        FundSourceAllocationID = x.FundSourceAllocationID,
        FundSourceAllocationName = x.FundSourceAllocationName,
        FundSourceID = x.FundSourceID,
        FundSourceNumber = x.FundSource.FundSourceNumber,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        AllocationAmount = x.AllocationAmount,
        DNRUplandRegionID = x.DNRUplandRegionID,
        DNRUplandRegionName = x.DNRUplandRegion != null ? x.DNRUplandRegion.DNRUplandRegionName : null,
        OrganizationID = x.OrganizationID,
        OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null,
        FundSourceAllocationPriorityName = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityNumber.ToString() : null,
        FundSourceAllocationPriorityColor = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityColor : null,
        HasFundFSPs = x.HasFundFSPs,
        ProjectCount = x.ProjectFundSourceAllocationRequests.Select(p => p.ProjectID).Distinct().Count()
    };

    public static readonly Expression<Func<FundSourceAllocation, FundSourceAllocationDetail>> AsDetail = x => new FundSourceAllocationDetail
    {
        FundSourceAllocationID = x.FundSourceAllocationID,
        FundSourceAllocationName = x.FundSourceAllocationName,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        AllocationAmount = x.AllocationAmount,
        HasFundFSPs = x.HasFundFSPs,
        LikelyToUse = x.LikelyToUse,
        FundSourceID = x.FundSourceID,
        FundSourceNumber = x.FundSource.FundSourceNumber,
        DNRUplandRegionID = x.DNRUplandRegionID,
        DNRUplandRegionName = x.DNRUplandRegion != null ? x.DNRUplandRegion.DNRUplandRegionName : null,
        OrganizationID = x.OrganizationID,
        OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null,
        FederalFundCodeID = x.FederalFundCodeID,
        FederalFundCodeName = x.FederalFundCode != null ? x.FederalFundCode.FederalFundCodeAbbrev : null,
        DivisionID = x.DivisionID,
        DivisionName = null, // Resolved client-side in GetByIDAsDetailAsync
        FundSourceManagerID = x.FundSourceManagerID,
        FundSourceManagerName = x.FundSourceManager != null ? x.FundSourceManager.FirstName + " " + x.FundSourceManager.LastName : null,
        FundSourceAllocationPriorityID = x.FundSourceAllocationPriorityID,
        FundSourceAllocationPriorityName = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityNumber.ToString() : null,
        FundSourceAllocationPriorityColor = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityColor : null,
        FundSourceAllocationSourceID = x.FundSourceAllocationSourceID,
        FundSourceAllocationSourceName = x.FundSourceAllocationSource != null ? x.FundSourceAllocationSource.FundSourceAllocationSourceDisplayName : null,
        ProjectCount = x.ProjectFundSourceAllocationRequests.Select(p => p.ProjectID).Distinct().Count(),
        AgreementCount = x.AgreementFundSourceAllocations.Select(a => a.AgreementID).Distinct().Count(),
        ProgramIndexCount = x.FundSourceAllocationProgramIndexProjectCodes.Where(p => p.ProgramIndexID != null).Select(p => p.ProgramIndexID).Distinct().Count(),
        ProjectCodeCount = x.FundSourceAllocationProgramIndexProjectCodes.Where(p => p.ProjectCodeID != null).Select(p => p.ProjectCodeID).Distinct().Count()
    };
}
