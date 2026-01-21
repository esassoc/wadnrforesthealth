using System.Linq;
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
        //MP 1/21/26 Because of the way EF Core translates queries, we have to specifically omit geometries for the union.
        //So only get what we need.
        AssociatedProjectsCount = x.ProjectOrganizations
            .Select(po => new
            {
                po.Project.ProjectID,
                po.Project.ProjectApprovalStatusID,
                po.Project.ProjectType.LimitVisibilityToAdmin
            })
            .Union(
                x.FundSourceAllocations
                    .SelectMany(fsa => fsa.ProjectFundSourceAllocationRequests)
                    .Select(r => new
                    {
                        r.Project.ProjectID,
                        r.Project.ProjectApprovalStatusID,
                        r.Project.ProjectType.LimitVisibilityToAdmin
                    }))
            .Where(p => p.ProjectApprovalStatusID == Projects.ApprovedStatusId && !p.LimitVisibilityToAdmin)
            .Select(p => p.ProjectID)
            .Distinct()
            .Count(),
        AssociatedFundSourcesCount = x.FundSources.Count,
        AssociatedUsersCount = x.People.Count
    };
}
