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
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        ConditionsAndRequirements = x.ConditionsAndRequirements,
        ComplianceNotes = x.ComplianceNotes,
        CFDANumber = x.CFDANumber,
        FundSourceTypeID = x.FundSourceTypeID,
        ShortName = x.ShortName,
        FundSourceStatusID = x.FundSourceStatusID,
        OrganizationID = x.OrganizationID,
        TotalAwardAmount = x.TotalAwardAmount
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
}
