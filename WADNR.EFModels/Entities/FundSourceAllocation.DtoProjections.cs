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
        FundSourceAllocationName = x.FundSource.FundSourceNumber + " " + x.FundSourceAllocationName,
        FundSourceName = x.FundSource.FundSourceName
    };

    public static readonly Expression<Func<FundSourceAllocation, FundSourceAllocationExcelRow>> AsExcelRow = x => new FundSourceAllocationExcelRow
    {
        FundSourceNumber = x.FundSource.FundSourceNumber,
        FundSourceAllocationName = x.FundSourceAllocationName,
        ProgramManagerNames = string.Join(", ",
            x.FundSourceAllocationProgramManagers
                .Select(pm => pm.Person.FirstName + " " + pm.Person.LastName)),
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        ParentFundSourceStatusName = null, // Resolved post-query via static lookup
        DNRUplandRegionName = x.DNRUplandRegion != null ? x.DNRUplandRegion.DNRUplandRegionName : null,
        FederalFundCodeDisplay = x.FederalFundCode != null
            ? x.FederalFundCode.FederalFundCodeAbbrev : null,
        AllocationAmount = x.AllocationAmount,
        ProgramIndexProjectCodeDisplay = string.Join(", ",
            x.FundSourceAllocationProgramIndexProjectCodes
                .Select(y => y.ProgramIndex.ProgramIndexCode
                    + (y.ProjectCode != null ? " / " + y.ProjectCode.ProjectCodeName : ""))),
        OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null
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
        ProjectCount = x.ProjectFundSourceAllocationRequests.Select(p => p.ProjectID).Distinct().Count(),
        FundSourceManagerName = x.FundSourceManager != null
            ? x.FundSourceManager.FirstName + " " + x.FundSourceManager.LastName
            : null,
        ProgramManagerNames = string.Join(", ",
            x.FundSourceAllocationProgramManagers
                .Select(pm => pm.Person.FirstName + " " + pm.Person.LastName)),
        FundSourceStatusID = x.FundSource.FundSourceStatusID,
        DivisionID = x.DivisionID,
        FederalFundCodeAbbrev = x.FederalFundCode != null
            ? x.FederalFundCode.FederalFundCodeAbbrev : null,
        ProgramIndexProjectCodeDisplay = string.Join(", ",
            x.FundSourceAllocationProgramIndexProjectCodes
                .Select(y => y.ProgramIndex.ProgramIndexCode
                    + (y.ProjectCode != null ? " / " + y.ProjectCode.ProjectCodeName : "")))
    };

    public static readonly Expression<Func<FundSourceAllocationBudgetLineItem, FundSourceAllocationBudgetLineItemGridRow>> AsBudgetLineItemGridRow = x => new FundSourceAllocationBudgetLineItemGridRow
    {
        FundSourceAllocationBudgetLineItemID = x.FundSourceAllocationBudgetLineItemID,
        CostTypeID = x.CostTypeID,
        FundSourceAllocationBudgetLineItemAmount = x.FundSourceAllocationBudgetLineItemAmount,
        FundSourceAllocationBudgetLineItemNote = x.FundSourceAllocationBudgetLineItemNote
    };

    public static readonly Expression<Func<ProjectFundSourceAllocationRequest, FundSourceAllocationProjectGridRow>> AsProjectGridRow = x => new FundSourceAllocationProjectGridRow
    {
        ProjectID = x.ProjectID,
        ProjectName = x.Project.ProjectName,
        FhtProjectNumber = x.Project.FhtProjectNumber,
        MatchAmount = x.MatchAmount,
        PayAmount = x.PayAmount,
        TotalAmount = x.TotalAmount
    };

    public static readonly Expression<Func<AgreementFundSourceAllocation, FundSourceAllocationAgreementGridRow>> AsAgreementGridRow = x => new FundSourceAllocationAgreementGridRow
    {
        AgreementID = x.AgreementID,
        AgreementNumber = x.Agreement.AgreementNumber,
        AgreementTitle = x.Agreement.AgreementTitle,
        AgreementTypeAbbrev = x.Agreement.AgreementType.AgreementTypeAbbrev,
        OrganizationID = x.Agreement.OrganizationID,
        OrganizationName = x.Agreement.Organization.OrganizationName,
        StartDate = x.Agreement.StartDate,
        EndDate = x.Agreement.EndDate,
        AgreementAmount = x.Agreement.AgreementAmount
    };

    public static readonly Expression<Func<FundSourceAllocationChangeLog, FundSourceAllocationChangeLogGridRow>> AsChangeLogGridRow = x => new FundSourceAllocationChangeLogGridRow
    {
        FundSourceAllocationChangeLogID = x.FundSourceAllocationChangeLogID,
        OldValue = x.FundSourceAllocationAmountOldValue,
        NewValue = x.FundSourceAllocationAmountNewValue,
        Note = x.FundSourceAllocationAmountNote,
        ChangePersonID = x.ChangePersonID,
        ChangePersonName = x.ChangePerson.FirstName + " " + x.ChangePerson.LastName,
        ChangeDate = x.ChangeDate
    };

    public static readonly Expression<Func<FundSourceAllocationFileResource, FundSourceAllocationFileGridRow>> AsFileGridRow = x => new FundSourceAllocationFileGridRow
    {
        FundSourceAllocationFileResourceID = x.FundSourceAllocationFileResourceID,
        FileResourceID = x.FileResourceID,
        FileResourceGUID = x.FileResource.FileResourceGUID,
        DisplayName = x.DisplayName,
        Description = x.Description,
        OriginalBaseFilename = x.FileResource.OriginalBaseFilename,
        FileResourceMimeTypeName = x.FileResource.FileResourceMimeType.FileResourceMimeTypeDisplayName,
        CreateDate = x.FileResource.CreateDate
    };

    public static readonly Expression<Func<FundSourceAllocationExpenditure, FundSourceAllocationExpenditureGridRow>> AsExpenditureGridRow = x => new FundSourceAllocationExpenditureGridRow
    {
        FundSourceAllocationExpenditureID = x.FundSourceAllocationExpenditureID,
        CostTypeID = x.CostTypeID,
        Biennium = x.Biennium,
        FiscalMonth = x.FiscalMonth,
        CalendarYear = x.CalendarYear,
        CalendarMonth = x.CalendarMonth,
        ExpenditureAmount = x.ExpenditureAmount
    };

    public static readonly Expression<Func<FundSourceAllocationProgramIndexProjectCode, FundSourceAllocationProgramIndexProjectCodeItem>> AsProgramIndexProjectCodeItem = x => new FundSourceAllocationProgramIndexProjectCodeItem
    {
        FundSourceAllocationProgramIndexProjectCodeID = x.FundSourceAllocationProgramIndexProjectCodeID,
        ProgramIndexID = x.ProgramIndexID,
        ProgramIndexCode = x.ProgramIndex.ProgramIndexCode,
        ProjectCodeID = x.ProjectCodeID,
        ProjectCodeName = x.ProjectCode != null ? x.ProjectCode.ProjectCodeName : null
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
        FundSourceName = x.FundSource.FundSourceName,
        FundSourceStatusID = x.FundSource.FundSourceStatusID,
        FundSourceTypeID = x.FundSource.FundSourceTypeID,
        FundSourceTypeName = x.FundSource.FundSourceType != null ? x.FundSource.FundSourceType.FundSourceTypeName : null,
        CFDANumber = x.FundSource.CFDANumber,
        DNRUplandRegionID = x.DNRUplandRegionID,
        DNRUplandRegionName = x.DNRUplandRegion != null ? x.DNRUplandRegion.DNRUplandRegionName : null,
        OrganizationID = x.OrganizationID,
        OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null,
        FederalFundCodeID = x.FederalFundCodeID,
        FederalFundCodeName = x.FederalFundCode != null ? x.FederalFundCode.FederalFundCodeAbbrev : null,
        DivisionID = x.DivisionID,
        DivisionName = null, // Resolved post-query in GetByIDAsDetailAsync
        FundSourceManagerID = x.FundSourceManagerID,
        FundSourceManagerName = x.FundSourceManager != null ? x.FundSourceManager.FirstName + " " + x.FundSourceManager.LastName : null,
        FundSourceAllocationPriorityID = x.FundSourceAllocationPriorityID,
        FundSourceAllocationPriorityName = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityNumber.ToString() : null,
        FundSourceAllocationPriorityColor = x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityColor : null,
        FundSourceAllocationSourceID = x.FundSourceAllocationSourceID,
        FundSourceAllocationSourceName = x.FundSourceAllocationSource != null ? x.FundSourceAllocationSource.FundSourceAllocationSourceDisplayName : null,
        ProjectCount = x.ProjectFundSourceAllocationRequests.Select(p => p.ProjectID).Distinct().Count(),
        AgreementCount = x.AgreementFundSourceAllocations.Select(a => a.AgreementID).Distinct().Count(),
        ProgramManagers = x.FundSourceAllocationProgramManagers
            .Select(pm => new PersonLookupItem { PersonID = pm.PersonID, FullName = pm.Person.FirstName + " " + pm.Person.LastName })
            .ToList(),
        LikelyToUsePeople = x.LikelyToUse == true
            ? x.FundSourceAllocationLikelyPeople
                .Select(lp => new PersonLookupItem { PersonID = lp.PersonID, FullName = lp.Person.FirstName + " " + lp.Person.LastName })
                .ToList()
            : new List<PersonLookupItem>(),
        ProgramIndexProjectCodes = x.FundSourceAllocationProgramIndexProjectCodes
            .Select(y => new FundSourceAllocationProgramIndexProjectCodeItem
            {
                FundSourceAllocationProgramIndexProjectCodeID = y.FundSourceAllocationProgramIndexProjectCodeID,
                ProgramIndexID = y.ProgramIndexID,
                ProgramIndexCode = y.ProgramIndex.ProgramIndexCode,
                ProjectCodeID = y.ProjectCodeID,
                ProjectCodeName = y.ProjectCode != null ? y.ProjectCode.ProjectCodeName : null
            })
            .ToList()
    };

    public static readonly Expression<Func<FundSourceAllocation, FundSourceAllocationApiJson>> AsApiJson = x => new FundSourceAllocationApiJson
    {
        FundSourceAllocationID = x.FundSourceAllocationID,
        FundSourceAllocationName = x.FundSourceAllocationName,
        FundSourceID = x.FundSourceID,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        AllocationAmount = x.AllocationAmount,
        FederalFundCodeID = x.FederalFundCodeID,
        FederalFundCodeName = x.FederalFundCode != null ? x.FederalFundCode.FederalFundCodeAbbrev : null,
        OrganizationID = x.OrganizationID,
        OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null,
        RegionID = x.DNRUplandRegionID,
        RegionName = x.DNRUplandRegion != null ? x.DNRUplandRegion.DNRUplandRegionName : null,
        DivisionID = x.DivisionID,
        DivisionName = null, // Resolved client-side via Division.AllLookupDictionary
        FundSourceManagerID = x.FundSourceManagerID,
        FundSourceManagerName = x.FundSourceManager != null ? x.FundSourceManager.FirstName + " " + x.FundSourceManager.LastName : null,
        FundSourceAllocationFileResourceIDs = x.FundSourceAllocationFileResources.Select(f => f.FileResourceID).ToList()
    };

    public static readonly Expression<Func<FundSourceAllocationProgramIndexProjectCode, FundSourceAllocationProgramIndexProjectCodeApiJson>> AsApiJsonProgramIndexProjectCode =
        x => new FundSourceAllocationProgramIndexProjectCodeApiJson
        {
            FundSourceAllocationProgramIndexProjectCodeID = x.FundSourceAllocationProgramIndexProjectCodeID,
            FundSourceAllocationID = x.FundSourceAllocationID,
            ProgramIndexID = x.ProgramIndexID,
            ProgramIndexCode = x.ProgramIndex.ProgramIndexCode,
            ProjectCodeID = x.ProjectCodeID,
            ProjectCodeName = x.ProjectCode != null ? x.ProjectCode.ProjectCodeName : null
        };
}
