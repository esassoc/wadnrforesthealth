using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectOrganizations
{
    public static async Task<List<ProjectOrganizationItem>> ListForProjectAsItemAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.ProjectOrganizations
            .AsNoTracking()
            .Where(o => o.ProjectID == projectID)
            .Select(ProjectOrganizationProjections.AsItem)
            .OrderBy(o => o.RelationshipTypeName)
            .ThenBy(o => o.OrganizationName)
            .ToListAsync();
    }

    public static async Task<List<ProjectOrganizationExcelRow>> ListAllAsExcelRowAsync(WADNRDbContext dbContext, List<int> projectIDs)
    {
        return await dbContext.ProjectOrganizations
            .AsNoTracking()
            .Where(o => projectIDs.Contains(o.ProjectID))
            .Select(o => new ProjectOrganizationExcelRow
            {
                ProjectID = o.ProjectID,
                ProjectName = o.Project.ProjectName,
                OrganizationID = o.OrganizationID,
                OrganizationName = o.Organization.OrganizationName,
                RelationshipTypeName = o.RelationshipType.RelationshipTypeName,
                PrimaryContactPersonName = o.Organization.PrimaryContactPerson != null
                    ? o.Organization.PrimaryContactPerson.FirstName + " " + o.Organization.PrimaryContactPerson.LastName
                    : null,
                OrganizationTypeName = o.Organization.OrganizationType.OrganizationTypeName
            })
            .OrderBy(o => o.ProjectID)
            .ThenBy(o => o.RelationshipTypeName)
            .ThenBy(o => o.OrganizationName)
            .ToListAsync();
    }

    public static async Task<List<ProjectOrganizationItem>> SaveAllAsync(WADNRDbContext dbContext, int projectID, ProjectOrganizationSaveRequest request)
    {
        var existing = await dbContext.ProjectOrganizations
            .Where(o => o.ProjectID == projectID)
            .ToListAsync();

        var requestIDs = request.Organizations
            .Where(r => r.ProjectOrganizationID.HasValue)
            .Select(r => r.ProjectOrganizationID!.Value)
            .ToHashSet();

        // Delete orgs not in request
        var toDelete = existing.Where(e => !requestIDs.Contains(e.ProjectOrganizationID)).ToList();
        dbContext.ProjectOrganizations.RemoveRange(toDelete);

        // Create new orgs (items with null ID)
        foreach (var item in request.Organizations.Where(r => !r.ProjectOrganizationID.HasValue))
        {
            dbContext.ProjectOrganizations.Add(new ProjectOrganization
            {
                ProjectID = projectID,
                OrganizationID = item.OrganizationID,
                RelationshipTypeID = item.RelationshipTypeID
            });
        }

        await dbContext.SaveChangesAsync();

        return await ListForProjectAsItemAsync(dbContext, projectID);
    }
}
