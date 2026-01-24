using System.Collections.Generic;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AgreementProjections
{
    public static readonly Expression<Func<Agreement, AgreementDetail>> AsDetail = x => new AgreementDetail
    {
        AgreementID = x.AgreementID,
        AgreementTitle = x.AgreementTitle,
        AgreementNumber = x.AgreementNumber,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        AgreementAmount = x.AgreementAmount,
        ExpendedAmount = x.ExpendedAmount,
        BalanceAmount = x.BalanceAmount,
        FirstBillDueOn = x.FirstBillDueOn,
        Notes = x.Notes,

        AgreementType = new AgreementTypeLookupItem
        {
            AgreementTypeID = x.AgreementType.AgreementTypeID,
            AgreementTypeName = x.AgreementType.AgreementTypeName
        },

        ContributingOrganization = new OrganizationLookupItem
        {
            OrganizationID = x.Organization.OrganizationID,
            OrganizationName = x.Organization.OrganizationName
        },

        AgreementStatus = x.AgreementStatus == null
            ? null
            : new AgreementStatusLookupItem
            {
                AgreementStatusID = x.AgreementStatus.AgreementStatusID,
                AgreementStatusName = x.AgreementStatus.AgreementStatusName
            },

        DNRUplandRegion = x.DNRUplandRegion == null
            ? null
            : new DNRUplandRegionLookupItem
            {
                DNRUplandRegionID = x.DNRUplandRegion.DNRUplandRegionID,
                DNRUplandRegionName = x.DNRUplandRegion.DNRUplandRegionName
            },

        FileResource = x.AgreementFileResource == null
            ? null
            : new FileResourceLookupItem
            {
                FileResourceID = x.AgreementFileResource.FileResourceID,
                FileResourceGUID = x.AgreementFileResource.FileResourceGUID,
                OriginalBaseFilename = x.AgreementFileResource.OriginalBaseFilename,
                OriginalFileExtension = x.AgreementFileResource.OriginalFileExtension
            },

        ProgramIndices = string.Join(", ", x.AgreementFundSourceAllocations
            .SelectMany(a => a.FundSourceAllocation.FundSourceAllocationProgramIndexProjectCodes)
            .Select(p => p.ProgramIndex.ProgramIndexCode)
            .Distinct()),

        ProjectCodes = string.Join(", ", x.AgreementFundSourceAllocations
            .SelectMany(a => a.FundSourceAllocation.FundSourceAllocationProgramIndexProjectCodes)
            .Where(p => p.ProjectCode != null)
            .Select(p => p.ProjectCode!.ProjectCodeName)
            .Distinct())
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

    public static readonly Expression<Func<AgreementPerson, AgreementContactGridRowRaw>> AsContactGridRowRaw = x => new AgreementContactGridRowRaw
    {
        AgreementPersonRoleID = x.AgreementPersonRoleID,
        PersonID = x.Person.PersonID,
        FirstName = x.Person.FirstName,
        LastName = x.Person.LastName,
        OrganizationID = x.Person.Organization == null ? null : x.Person.Organization.OrganizationID,
        OrganizationName = x.Person.Organization == null ? null : x.Person.Organization.DisplayName
    };

    // NOTE: Do not add a projection that reads `AgreementPerson.AgreementPersonRole` here; it's a computed lookup
    // property and EF can't translate it to SQL.

    public static AgreementContactGridRow ToContactGridRow(AgreementContactGridRowRaw x) => new()
    {
        Person = new PersonFirstNameLastName
        {
            PersonID = x.PersonID,
            FirstName = x.FirstName,
            LastName = x.LastName
        },
        AgreementRole = new AgreementPersonRoleLookupItem
        {
            AgreementPersonRoleID = x.AgreementPersonRoleID,
            AgreementPersonRoleName = AgreementPersonRole.AllLookupDictionary[x.AgreementPersonRoleID].AgreementPersonRoleDisplayName
        },
        ContributingOrganization = x.OrganizationID == null
            ? null
            : new OrganizationLookupItem
            {
                OrganizationID = x.OrganizationID.Value,
                OrganizationName = x.OrganizationName ?? string.Empty
            }
    };
}
