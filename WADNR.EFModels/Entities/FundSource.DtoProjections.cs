using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceProjections
{
    public static readonly Expression<Func<FundSource, FundSourceDetail>> AsDetail = x => new FundSourceDetail
    {
        FundSourceID = x.FundSourceID,
        FundSourceName = x.FundSourceName,
        FundSourceNumber = x.FundSourceNumber,
        ShortName = x.ShortName,
        FundSourceTitle = string.IsNullOrWhiteSpace(x.ShortName) ? x.FundSourceName : x.FundSourceName + " (" + x.ShortName + ")",

        // Organization
        Organization = new OrganizationLookupItem
        {
            OrganizationID = x.Organization.OrganizationID,
            OrganizationName = x.Organization.OrganizationName
        },

        // Status
        FundSourceStatus = x.FundSourceStatus == null
            ? null
            : new FundSourceStatusLookupItem
            {
                FundSourceStatusID = x.FundSourceStatus.FundSourceStatusID,
                FundSourceStatusName = x.FundSourceStatus.FundSourceStatusName
            },

        // Type
        FundSourceTypeName = x.FundSourceType == null ? null : x.FundSourceType.FundSourceTypeName,

        // Financial
        TotalAwardAmount = x.TotalAwardAmount,
        CurrentBalance = x.FundSourceAllocations.SelectMany(a => a.FundSourceAllocationBudgetLineItems).Sum(b => b.FundSourceAllocationBudgetLineItemAmount)
            - x.FundSourceAllocations.SelectMany(a => a.FundSourceAllocationExpenditures).Sum(e => e.ExpenditureAmount),
        CFDANumber = x.CFDANumber,

        // Dates
        StartDate = x.StartDate,
        EndDate = x.EndDate,

        // Additional details
        ConditionsAndRequirements = x.ConditionsAndRequirements,
        ComplianceNotes = x.ComplianceNotes,

        // Counts
        AllocationCount = x.FundSourceAllocations.Count,
        AgreementCount = x.FundSourceAllocations.SelectMany(a => a.AgreementFundSourceAllocations).Select(a => a.AgreementID).Distinct().Count(),
        ProjectCount = x.FundSourceAllocations.SelectMany(a => a.ProjectFundSourceAllocationRequests).Select(p => p.ProjectID).Distinct().Count(),
        FileCount = x.FundSourceFileResources.Count,
        NoteCount = x.FundSourceNotes.Count,
        InternalNoteCount = x.FundSourceNoteInternals.Count
    };

    public static readonly Expression<Func<FundSource, FundSourceGridRow>> AsGridRow = x => new FundSourceGridRow
    {
        FundSourceID = x.FundSourceID,
        FundSourceName = x.FundSourceName,
        FundSourceNumber = x.FundSourceNumber,
        ShortName = x.ShortName,
        TotalAwardAmount = x.TotalAwardAmount,
        CFDANumber = x.CFDANumber,
        FundSourceTitle = string.IsNullOrWhiteSpace(x.ShortName) ? x.FundSourceName : $"{x.FundSourceName} ({x.ShortName})",
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        FundSourceStatusName = x.FundSourceStatus == null? null : x.FundSourceStatus.FundSourceStatusName,
        FundSourceTypeDisplay = x.FundSourceType == null ? null : x.FundSourceType.FundSourceTypeName
    };

    public static readonly Expression<Func<FundSource, FundSourceLookupItem>> AsLookupItem = x => new FundSourceLookupItem
    {
        FundSourceID = x.FundSourceID,
        FundSourceNumber = x.FundSourceNumber,
        FundSourceName = x.FundSourceName
    };
}
