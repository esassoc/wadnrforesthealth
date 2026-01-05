using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AgreementProjections
{
    public static readonly Expression<Func<Agreement, AgreementDetail>> AsDetail = x => new AgreementDetail
    {
        AgreementID = x.AgreementID,
        AgreementTypeID = x.AgreementTypeID,
        AgreementTitle = x.AgreementTitle,
        AgreementNumber = x.AgreementNumber,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        AgreementAmount = x.AgreementAmount,
        ExpendedAmount = x.ExpendedAmount,
        BalanceAmount = x.BalanceAmount,
        DNRUplandRegionID = x.DNRUplandRegionID,
        FirstBillDueOn = x.FirstBillDueOn,
        Notes = x.Notes,
        OrganizationID = x.OrganizationID,
        AgreementStatusID = x.AgreementStatusID,
        AgreementFileResourceID = x.AgreementFileResourceID,
        AgreementTypeName = x.AgreementType.AgreementTypeName,
        AgreementStatusName = x.AgreementStatus == null ? null : x.AgreementStatus.AgreementStatusName,
        OrganizationName = x.Organization.OrganizationName,
        DNRUplandRegionName = x.DNRUplandRegion == null ? null : x.DNRUplandRegion.DNRUplandRegionName
    };

    public static readonly Expression<Func<Agreement, AgreementGridRow>> AsGridRow = x => new AgreementGridRow
    {
        AgreementID = x.AgreementID,
        AgreementTitle = x.AgreementTitle,
        AgreementNumber = x.AgreementNumber,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        AgreementAmount = x.AgreementAmount,
        ExpendedAmount = x.ExpendedAmount,
        BalanceAmount = x.BalanceAmount,
        AgreementTypeAbbrev = x.AgreementType.AgreementTypeAbbrev,
        AgreementStatusName = x.AgreementStatus == null ? null : x.AgreementStatus.AgreementStatusName,
        Organization = new OrganizationLookupItem
        {
            OrganizationID = x.Organization.OrganizationID,
            OrganizationName = x.Organization.OrganizationName
        },
        FundSources = x.AgreementFundSourceAllocations
            .Select(a => new FundSourceLookupItem
            {
                FundSourceID = a.FundSourceAllocation.FundSourceID,
                FundSourceNumber = a.FundSourceAllocation.FundSource.FundSourceNumber
            })
            .Distinct()
            .OrderBy(fs => fs.FundSourceNumber)
            .ToList(),
        ProgramIndices = string.Join(", ", x.AgreementFundSourceAllocations
            .SelectMany(a => a.FundSourceAllocation.FundSourceAllocationProgramIndexProjectCodes)
            .Select(p => p.ProgramIndex.ProgramIndexCode)
            .Distinct()),
        ProjectCodes = string.Join(", ", x.AgreementFundSourceAllocations
            .SelectMany(a => a.FundSourceAllocation.FundSourceAllocationProgramIndexProjectCodes)
            .Where(p => p.ProjectCode != null)
            .Select(p => p.ProjectCode.ProjectCodeName)
            .Distinct())
    };
}
