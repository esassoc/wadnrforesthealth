using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class People
{
    public static async Task<List<PersonLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        var items = await dbContext.People
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(PersonProjections.AsLookupItem)
            .ToListAsync();
        return items;
    }

    public static async Task<List<PersonGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        // First, get the base data
        var people = await dbContext.People
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(PersonProjections.AsGridRow)
            .ToListAsync();

        // Get all person roles in one query
        var personRoles = await dbContext.PersonRoles
            .AsNoTracking()
            .ToListAsync();

        // Map roles to each person
        var roleLookup = Role.AllLookupDictionary;
        foreach (var person in people)
        {
            var roles = personRoles
                .Where(pr => pr.PersonID == person.PersonID)
                .Select(pr => roleLookup.TryGetValue(pr.RoleID, out var role) ? role : null)
                .Where(r => r != null)
                .ToList();

            var baseRole = roles.FirstOrDefault(r => r!.IsBaseRole);
            var supplementalRoles = roles.Where(r => !r!.IsBaseRole).ToList();

            person.RoleName = baseRole?.RoleDisplayName;
            person.SupplementalRoles = supplementalRoles.Any()
                ? string.Join(", ", supplementalRoles.Select(r => r!.RoleDisplayName))
                : null;
        }

        return people;
    }

    public static async Task<PersonDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int personID)
    {
        var person = await dbContext.People
            .AsNoTracking()
            .Where(x => x.PersonID == personID)
            .Select(PersonProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (person == null) return null;

        // Get person roles and resolve from static lookup
        var personRoleIDs = await dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => pr.PersonID == personID)
            .Select(pr => pr.RoleID)
            .ToListAsync();

        var roleLookup = Role.AllLookupDictionary;
        var roles = personRoleIDs
            .Select(roleID => roleLookup.TryGetValue(roleID, out var role) ? role : null)
            .Where(r => r != null)
            .ToList();

        var baseRole = roles.FirstOrDefault(r => r!.IsBaseRole);
        var supplementalRoles = roles.Where(r => !r!.IsBaseRole).ToList();

        person.BaseRoleName = baseRole?.RoleDisplayName;
        person.SupplementalRoles = supplementalRoles.Any()
            ? string.Join(", ", supplementalRoles.Select(r => r!.RoleDisplayName))
            : null;

        // A "full user" has a base role that is not Unassigned
        person.IsFullUser = baseRole != null && baseRole != Role.Unassigned;

        return person;
    }

    public static PersonDetail? GetByEmailAsDetail(WADNRDbContext dbContext, string email)
    {
        var person = dbContext.People
            .AsNoTracking()
            .Where(x => x.Email == email)
            .Select(PersonProjections.AsDetail)
            .SingleOrDefault();
        return person;
    }
}