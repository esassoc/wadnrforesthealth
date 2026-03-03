using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.Helpers;

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

        if (person == null)
        {
            return null;
        }

        // Get person roles and resolve from static lookup
        var personRoleIDs = await dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => pr.PersonID == personID)
            .Select(pr => pr.RoleID)
            .ToListAsync();

        PopulateRoles(person, personRoleIDs);

        return person;
    }

    private static void PopulateRoles(PersonDetail person, List<int> personRoleIDs)
    {
        var roleLookup = Role.AllLookupDictionary;
        var roles = personRoleIDs
            .Select(roleID => roleLookup.TryGetValue(roleID, out var role) ? role : null)
            .Where(r => r != null)
            .ToList();

        var baseRole = roles.FirstOrDefault(r => r!.IsBaseRole);
        var supplementalRoles = roles.Where(r => !r!.IsBaseRole).ToList();

        person.BaseRole = baseRole != null ? new RoleLookupItem
        {
            RoleID = baseRole.RoleID,
            RoleName = baseRole.RoleDisplayName
        } : null;

        person.SupplementalRoleList = supplementalRoles
            .Select(r => new RoleLookupItem { RoleID = r!.RoleID, RoleName = r.RoleDisplayName })
            .ToList();

        person.SupplementalRoles = supplementalRoles.Any()
            ? string.Join(", ", supplementalRoles.Select(r => r!.RoleDisplayName))
            : null;

        // A "full user" has a base role that is not Unassigned
        person.IsFullUser = baseRole != null && baseRole != Role.Unassigned;
    }

    public static async Task<PersonDetail?> GetByGlobalIDAsDetailAsync(WADNRDbContext dbContext, string globalID)
    {
        var person = await dbContext.People
            .AsNoTracking().Where(x => x.GlobalID == globalID).Select(PersonProjections.AsDetail).SingleOrDefaultAsync();
        if (person == null)
        {
            return null;
        }

        // Get person roles and resolve from static lookup
        var personRoleIDs = await dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => pr.PersonID == person.PersonID)
            .Select(pr => pr.RoleID)
            .ToListAsync();

        PopulateRoles(person, personRoleIDs);

        return person;
    }

    public static PersonDetail? GetByGlobalIDAsDetail(WADNRDbContext dbContext, string globalID)
    {
        var person = dbContext.People
            .AsNoTracking().Where(x => x.GlobalID == globalID).Select(PersonProjections.AsDetail).SingleOrDefault();
        if (person == null)
        {
            return null;
        }

        // Get person roles and resolve from static lookup
        var personRoleIDs = dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => pr.PersonID == person.PersonID)
            .Select(pr => pr.RoleID)
            .ToList();

        PopulateRoles(person, personRoleIDs);

        return person;
    }

    public static async Task<PersonDetail?> UpdatePrimaryContactOrganizationsAsync(WADNRDbContext dbContext, int personID, PersonPrimaryContactOrganizationsUpdateRequest dto)
    {
        // Clear all orgs where this person is the primary contact
        await dbContext.Organizations
            .Where(o => o.PrimaryContactPersonID == personID)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.PrimaryContactPersonID, (int?)null));

        // Set this person as primary contact on the new list
        if (dto.OrganizationIDs.Count > 0)
        {
            await dbContext.Organizations
                .Where(o => dto.OrganizationIDs.Contains(o.OrganizationID))
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.PrimaryContactPersonID, personID));
        }

        return await GetByIDAsDetailAsync(dbContext, personID);
    }

    public static async Task<PersonDetail?> UpdateClaims(WADNRDbContext dbContext, ClaimsPrincipal claimsPrincipal)
    {
        int? personID = null;
        var globalID = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Sub)?.Value;
        if (!string.IsNullOrEmpty(globalID))
        {
            personID = await dbContext.People.AsNoTracking().Where(x => x.GlobalID == globalID).Select(x => x.PersonID).SingleOrDefaultAsync();
        }

        Person person;
        var email = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Emails)?.Value;
        if (personID is > 0)
        {
            person = await dbContext.People.FirstOrDefaultAsync(x => x.PersonID == personID);
        }
        else
        {
            person = await dbContext.People.FirstOrDefaultAsync(x => x.Email == email);
        }

        if (person == null)
        {
            person = new Person
            {
                GlobalID = globalID,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                WebServiceAccessToken = Guid.NewGuid(),
                ReceiveSupportEmails = false,
            };
            var personRole = new PersonRole { Person = person, RoleID = (int)RoleEnum.Unassigned };

            dbContext.People.Add(person);
            dbContext.PersonRoles.Add(personRole);
        }

        var firstName = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.GivenName)?.Value;
        var lastName = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.FamilyName)?.Value;

        if (!string.IsNullOrEmpty(globalID))
        {
            person.GlobalID = globalID;
        }

        if (!string.IsNullOrEmpty(firstName))
        {
            person.FirstName = firstName;
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            person.LastName = lastName;
        }

        if (!string.IsNullOrEmpty(email))
        {
            person.Email = email;
        }

        await dbContext.SaveChangesWithNoAuditingAsync();
        await dbContext.Entry(person).ReloadAsync();

        return await GetByIDAsDetailAsync(dbContext, person.PersonID);
    }

}

public static class PersonDetailExtensions
{
    public static bool IsAnonymousOrUnassigned(this PersonDetail? user) =>
        user == null ||
        user.PersonID == PersonDetail.AnonymousPersonID ||
        (user.BaseRole != null && user.BaseRole.RoleID == (int)RoleEnum.Unassigned);

    public static bool HasElevatedProjectAccess(this PersonDetail? user) =>
        user?.BaseRole != null &&
        (user.BaseRole.RoleID == (int)RoleEnum.Admin ||
         user.BaseRole.RoleID == (int)RoleEnum.EsaAdmin ||
         user.BaseRole.RoleID == (int)RoleEnum.ProjectSteward);

    public static bool HasCanEditProgramRole(this PersonDetail? user) =>
        user?.SupplementalRoleList?.Any(r => r.RoleID == (int)RoleEnum.CanEditProgram) ?? false;

    public static bool CanViewAdminLimitedProjects(this PersonDetail? user) =>
        user.HasElevatedProjectAccess() || user.HasCanEditProgramRole();
}