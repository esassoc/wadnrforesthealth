using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class OrganizationProjections
{
    public static readonly Expression<Func<Organization, OrganizationDetail>> AsDetail = x => new OrganizationDetail
    {
        OrganizationID = x.OrganizationID,
        OrganizationGuid = x.OrganizationGuid,
        OrganizationName = x.OrganizationName,
        OrganizationAbbreviation = x.OrganizationShortName,
        SectorID = x.OrganizationTypeID,
        PrimaryContactPersonID = x.PrimaryContactPersonID,
        IsActive = x.IsActive,
        OrganizationUrl = x.OrganizationUrl,
        LogoFileResourceInfoID = x.LogoFileResourceID,
        IsUserAccountOrganization = x.IsEditable
    };

    public static readonly Expression<Func<Organization, OrganizationGridRow>> AsGridRow = x => new OrganizationGridRow
    {
        OrganizationID = x.OrganizationID,
        OrganizationName = x.OrganizationName,
        OrganizationShortName = x.OrganizationShortName,
        IsActive = x.IsActive,
        OrganizationTypeName = x.OrganizationType.OrganizationTypeName,
        AssociatedProjectsCount = x.ProjectOrganizations.Count,
        AssociatedFundSourcesCount = x.FundSources.Count,
        AssociatedUsersCount = x.People.Count
    };
}
