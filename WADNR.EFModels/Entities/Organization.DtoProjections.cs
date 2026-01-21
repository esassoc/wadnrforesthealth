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
        OrganizationShortName = x.OrganizationShortName,
        OrganizationTypeID = x.OrganizationTypeID,
        OrganizationTypeName = x.OrganizationType.OrganizationTypeName,
        PrimaryContactPersonID = x.PrimaryContactPersonID,
        PrimaryContactPersonFullName = x.PrimaryContactPerson != null
            ? x.PrimaryContactPerson.FirstName + " " + x.PrimaryContactPerson.LastName
            : null,
        PrimaryContactPersonOrganization = x.PrimaryContactPerson != null && x.PrimaryContactPerson.Organization != null
            ? x.PrimaryContactPerson.Organization.OrganizationName
            : null,
        IsActive = x.IsActive,
        OrganizationUrl = x.OrganizationUrl,
        LogoFileResourceID = x.LogoFileResourceID,
        LogoFileResourceUrl = x.LogoFileResource != null
            ? x.LogoFileResource.FileResourceGUID.ToString()
            : null,
        VendorID = x.VendorID,
        VendorName = x.Vendor != null ? x.Vendor.VendorName : null,
        VendorNumber = x.Vendor != null
            ? x.Vendor.StatewideVendorNumber + "-" + x.Vendor.StatewideVendorNumberSuffix
            : null,
        IsEditable = x.IsEditable,
        HasOrganizationBoundary = x.OrganizationBoundary != null,
        FundSourceAllocations = x.FundSourceAllocations
            .OrderBy(f => f.FundSourceAllocationName)
            .Select(f => new FundSourceAllocationLookupItem
            {
                FundSourceAllocationID = f.FundSourceAllocationID,
                FundSourceAllocationName = f.FundSourceAllocationName ?? string.Empty
            }).ToList(),
        People = x.People
            .Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new PersonWithStatus
            {
                PersonID = p.PersonID,
                FullNameFirstLast = p.FirstName + " " + p.LastName,
                IsActive = p.IsActive
            }).ToList(),
        NumberOfStewardedProjects = x.ProjectOrganizations
            .Count(po => po.RelationshipType.CanStewardProjects),
        NumberOfLeadImplementedProjects = x.ProjectOrganizations
            .Count(po => po.RelationshipType.IsPrimaryContact),
        NumberOfProjectsContributedTo = x.ProjectOrganizations.Count
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
