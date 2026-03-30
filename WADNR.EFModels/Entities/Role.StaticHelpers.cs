using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Roles
{
    public static async Task<List<RoleGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        // Get count of people per role
        var peopleCounts = await dbContext.PersonRoles
            .AsNoTracking()
            .GroupBy(pr => pr.RoleID)
            .Select(g => new { RoleID = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoleID, x => x.Count);

        return Role.All
            .Select(r => new RoleGridRow
            {
                RoleID = r.RoleID,
                RoleName = r.RoleName,
                RoleDisplayName = r.RoleDisplayName,
                RoleDescription = r.RoleDescription,
                IsBaseRole = r.IsBaseRole,
                PeopleCount = peopleCounts.TryGetValue(r.RoleID, out var count) ? count : 0
            })
            .OrderBy(r => r.RoleDisplayName)
            .ToList();
    }

    public static async Task<RoleDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int roleID)
    {
        if (!Role.AllLookupDictionary.TryGetValue(roleID, out var role))
        {
            return null;
        }

        var people = await dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => pr.RoleID == roleID)
            .Select(pr => new PersonLookupItem
            {
                PersonID = pr.Person.PersonID,
                FullName = pr.Person.FirstName + " " + (pr.Person.LastName ?? "")
            })
            .OrderBy(p => p.FullName)
            .ToListAsync();

        return new RoleDetail
        {
            RoleID = role.RoleID,
            RoleName = role.RoleName,
            RoleDisplayName = role.RoleDisplayName,
            RoleDescription = role.RoleDescription,
            IsBaseRole = role.IsBaseRole,
            PeopleCount = people.Count,
            People = people
        };
    }

    public static async Task<List<PersonLookupItem>> ListPeopleForRoleAsync(WADNRDbContext dbContext, int roleID)
    {
        return await dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => pr.RoleID == roleID)
            .Select(pr => new PersonLookupItem
            {
                PersonID = pr.Person.PersonID,
                FullName = pr.Person.FirstName + " " + (pr.Person.LastName ?? "")
            })
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }
}
