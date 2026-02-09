using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectPeople
{
    public static async Task<List<ProjectPersonItem>> ListForProjectAsItemAsync(WADNRDbContext dbContext, int projectID)
    {
        var people = await dbContext.ProjectPeople
            .AsNoTracking()
            .Include(pp => pp.Person)
            .Where(pp => pp.ProjectID == projectID)
            .ToListAsync();

        return people
            .Select(ProjectPersonProjections.ToItem)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.PersonFullName)
            .ToList();
    }

    public static async Task<List<ProjectPersonItem>> SaveAllAsync(WADNRDbContext dbContext, int projectID, ProjectContactSaveRequest request)
    {
        var existing = await dbContext.ProjectPeople
            .Where(pp => pp.ProjectID == projectID)
            .ToListAsync();

        var requestIDs = request.Contacts
            .Where(r => r.ProjectPersonID.HasValue)
            .Select(r => r.ProjectPersonID!.Value)
            .ToHashSet();

        // Delete contacts not in request
        var toDelete = existing.Where(e => !requestIDs.Contains(e.ProjectPersonID)).ToList();
        dbContext.ProjectPeople.RemoveRange(toDelete);

        // Create new contacts (items with null ID)
        foreach (var item in request.Contacts.Where(r => !r.ProjectPersonID.HasValue))
        {
            dbContext.ProjectPeople.Add(new ProjectPerson
            {
                ProjectID = projectID,
                PersonID = item.PersonID,
                ProjectPersonRelationshipTypeID = item.ProjectPersonRelationshipTypeID
            });
        }

        await dbContext.SaveChangesAsync();

        return await ListForProjectAsItemAsync(dbContext, projectID);
    }
}
