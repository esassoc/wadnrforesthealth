using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.Helpers;

namespace WADNR.EFModels.Entities;

public static class People
{

    public static async Task<List<PersonLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext, bool wadnrOnly = false)
    {
        const int wadnrOrganizationID = 4704;
        var query = dbContext.People
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (wadnrOnly)
        {
            query = query.Where(x => x.OrganizationID == wadnrOrganizationID);
        }

        var items = await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(PersonProjections.AsLookupItem)
            .ToListAsync();
        return items;
    }

    public static async Task<List<PersonWithOrganizationLookupItem>> ListWadnrAsLookupItemAsync(WADNRDbContext dbContext)
    {
        const int wadnrOrganizationID = 4704;
        var items = await dbContext.People
            .AsNoTracking()
            .Where(x => x.IsActive && x.OrganizationID == wadnrOrganizationID)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(PersonProjections.AsLookupItemWithOrganization)
            .ToListAsync();
        return items;
    }

    public static async Task<List<PersonGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
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
                ? string.Join(" | ", supplementalRoles.Select(r => r!.RoleDisplayName))
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
            ? string.Join(" | ", supplementalRoles.Select(r => r!.RoleDisplayName))
            : null;
    }

    public static PersonDetail? GetByIDAsDetail(WADNRDbContext dbContext, int personID)
    {
        var person = dbContext.People
            .AsNoTracking()
            .Where(x => x.PersonID == personID)
            .Select(PersonProjections.AsDetail)
            .SingleOrDefault();

        if (person == null)
        {
            return null;
        }

        var personRoleIDs = dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => pr.PersonID == personID)
            .Select(pr => pr.RoleID)
            .ToList();

        PopulateRoles(person, personRoleIDs);

        return person;
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

    public static async Task<PersonDetail?> CreateAsync(WADNRDbContext dbContext, PersonUpsertRequest request, int addedByPersonID)
    {
        var person = new Person
        {
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PersonAddress = request.PersonAddress,
            OrganizationID = request.OrganizationID,
            VendorID = request.VendorID,
            Notes = request.Notes,
            IsUser = request.IsUser,
            IsActive = true,
            ReceiveSupportEmails = false,
            CreateDate = DateTime.UtcNow,
            AddedByPersonID = addedByPersonID,
        };

        dbContext.People.Add(person);

        var personRole = new PersonRole { Person = person, RoleID = (int)RoleEnum.Unassigned };
        dbContext.PersonRoles.Add(personRole);

        await dbContext.SaveChangesAsync();

        return await GetByIDAsDetailAsync(dbContext, person.PersonID);
    }

    public static async Task<PersonDetail?> UpdateAsync(WADNRDbContext dbContext, int personID, PersonUpsertRequest request)
    {
        var person = await dbContext.People.FirstOrDefaultAsync(x => x.PersonID == personID);
        if (person == null) return null;

        // Only update name/email for contacts (non-users); users manage these via SSO
        if (!person.IsUser)
        {
            person.FirstName = request.FirstName;
            person.MiddleName = request.MiddleName;
            person.LastName = request.LastName;
            person.Email = request.Email;
        }

        person.OrganizationID = request.OrganizationID;
        person.VendorID = request.VendorID;
        person.PersonAddress = request.PersonAddress;
        person.Phone = request.Phone;
        person.Notes = request.Notes;
        person.UpdateDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return await GetByIDAsDetailAsync(dbContext, person.PersonID);
    }

    public static async Task<PersonDetail?> UpdateRolesAsync(WADNRDbContext dbContext, int personID, PersonRolesUpsertRequest request)
    {
        var person = await dbContext.People.FirstOrDefaultAsync(x => x.PersonID == personID);
        if (person == null) return null;

        // Validate: BaseRoleID must be a valid base role
        if (!Role.AllLookupDictionary.TryGetValue(request.BaseRoleID, out var baseRole) || !baseRole.IsBaseRole)
        {
            throw new InvalidOperationException("A valid base role is required.");
        }

        // Validate: if Unassigned, no supplemental roles allowed
        if (request.BaseRoleID == (int)RoleEnum.Unassigned && request.SupplementalRoleIDs.Count > 0)
        {
            throw new InvalidOperationException("Contacts with Unassigned base role cannot have supplemental roles.");
        }

        // Get current roles to detect removals
        var currentRoles = await dbContext.PersonRoles
            .Where(pr => pr.PersonID == personID)
            .ToListAsync();

        var currentRoleIDs = currentRoles.Select(pr => pr.RoleID).ToHashSet();

        // If removing ProjectSteward, clean up steward assignments
        if (currentRoleIDs.Contains((int)RoleEnum.ProjectSteward) && request.BaseRoleID != (int)RoleEnum.ProjectSteward)
        {
            var stewardRegions = await dbContext.PersonStewardRegions.Where(x => x.PersonID == personID).ToListAsync();
            dbContext.PersonStewardRegions.RemoveRange(stewardRegions);

            var stewardBranches = await dbContext.PersonStewardTaxonomyBranches.Where(x => x.PersonID == personID).ToListAsync();
            dbContext.PersonStewardTaxonomyBranches.RemoveRange(stewardBranches);

            var stewardOrgs = await dbContext.PersonStewardOrganizations.Where(x => x.PersonID == personID).ToListAsync();
            dbContext.PersonStewardOrganizations.RemoveRange(stewardOrgs);
        }

        // If removing CanEditProgram, clean up program assignments
        if (currentRoleIDs.Contains((int)RoleEnum.CanEditProgram) && !request.SupplementalRoleIDs.Contains((int)RoleEnum.CanEditProgram))
        {
            var programPeople = await dbContext.ProgramPeople.Where(x => x.PersonID == personID).ToListAsync();
            dbContext.ProgramPeople.RemoveRange(programPeople);
        }

        // Remove existing roles and add new ones
        dbContext.PersonRoles.RemoveRange(currentRoles);

        var newRoles = new List<PersonRole>
        {
            new PersonRole { PersonID = personID, RoleID = request.BaseRoleID }
        };
        foreach (var supplementalRoleID in request.SupplementalRoleIDs)
        {
            newRoles.Add(new PersonRole { PersonID = personID, RoleID = supplementalRoleID });
        }
        dbContext.PersonRoles.AddRange(newRoles);

        person.ReceiveSupportEmails = request.ReceiveSupportEmails;

        await dbContext.SaveChangesAsync();

        return await GetByIDAsDetailAsync(dbContext, personID);
    }

    public static async Task<PersonDetail?> ToggleActiveAsync(WADNRDbContext dbContext, int personID)
    {
        var person = await dbContext.People
            .Include(x => x.Organizations)
            .FirstOrDefaultAsync(x => x.PersonID == personID);
        if (person == null) return null;

        if (person.IsActive)
        {
            // Deactivating — validate not a primary contact
            if (person.Organizations.Any())
            {
                throw new InvalidOperationException("Cannot deactivate a person who is the primary contact for one or more organizations. Remove them as primary contact first.");
            }
            person.IsActive = false;
            person.ReceiveSupportEmails = false;
        }
        else
        {
            person.IsActive = true;
        }

        person.UpdateDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return await GetByIDAsDetailAsync(dbContext, personID);
    }

    public static async Task DeleteAsync(WADNRDbContext dbContext, int personID)
    {
        // Validate not a user (only contacts can be deleted)
        var isUser = await dbContext.People
            .AsNoTracking()
            .Where(p => p.PersonID == personID)
            .Select(p => p.IsUser)
            .SingleOrDefaultAsync();

        if (isUser)
        {
            throw new InvalidOperationException("Cannot delete a user. Only contacts can be deleted.");
        }

        // Delete related records
        var personRoles = await dbContext.PersonRoles.Where(x => x.PersonID == personID).ToListAsync();
        dbContext.PersonRoles.RemoveRange(personRoles);

        var allowedAuth = await dbContext.PersonAllowedAuthenticators.Where(x => x.PersonID == personID).ToListAsync();
        dbContext.PersonAllowedAuthenticators.RemoveRange(allowedAuth);

        var agreementPeople = await dbContext.AgreementPeople.Where(x => x.PersonID == personID).ToListAsync();
        dbContext.AgreementPeople.RemoveRange(agreementPeople);

        var interactionContacts = await dbContext.InteractionEventContacts.Where(x => x.PersonID == personID).ToListAsync();
        dbContext.InteractionEventContacts.RemoveRange(interactionContacts);

        var projectPeople = await dbContext.ProjectPeople.Where(x => x.PersonID == personID).ToListAsync();
        dbContext.ProjectPeople.RemoveRange(projectPeople);

        var notifications = await dbContext.Notifications.Where(x => x.PersonID == personID).ToListAsync();
        dbContext.Notifications.RemoveRange(notifications);

        // Clear primary contact references
        await dbContext.Organizations
            .Where(o => o.PrimaryContactPersonID == personID)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.PrimaryContactPersonID, (int?)null));

        var person = await dbContext.People.FirstOrDefaultAsync(x => x.PersonID == personID);
        if (person != null)
        {
            dbContext.People.Remove(person);
        }

        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<NotificationGridRow>> ListNotificationsForPersonAsGridRowAsync(WADNRDbContext dbContext, int personID)
    {
        var rawNotifications = await dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.PersonID == personID)
            .Select(n => new
            {
                n.NotificationID,
                n.NotificationDate,
                n.NotificationTypeID,
                ProjectCount = n.NotificationProjects.Count,
                FirstProject = n.NotificationProjects
                    .OrderBy(np => np.ProjectID)
                    .Select(np => new { np.ProjectID, np.Project.ProjectName })
                    .FirstOrDefault()
            })
            .OrderByDescending(n => n.NotificationDate)
            .ToListAsync();

        var notifications = rawNotifications.Select(n =>
        {
            var typeName = NotificationType.AllLookupDictionary.TryGetValue(n.NotificationTypeID, out var notifType)
                ? notifType.NotificationTypeDisplayName
                : $"Unknown ({n.NotificationTypeID})";

            var projectName = n.FirstProject?.ProjectName;
            var description = BuildNotificationDescription(n.NotificationTypeID, projectName);

            return new NotificationGridRow
            {
                NotificationID = n.NotificationID,
                NotificationDate = n.NotificationDate,
                NotificationTypeDisplayName = typeName,
                Description = description,
                ProjectCount = n.ProjectCount,
                ProjectID = n.ProjectCount == 1 ? n.FirstProject?.ProjectID : null,
                ProjectName = n.ProjectCount == 1 ? projectName : null,
            };
        }).ToList();

        return notifications;
    }

    private static string BuildNotificationDescription(int notificationTypeID, string? projectName)
    {
        var projectRef = !string.IsNullOrEmpty(projectName) ? projectName : "a project";
        return notificationTypeID switch
        {
            1 => "Project Update reminder sent.", // ProjectUpdateReminder
            2 => $"The update for Project {projectRef} was submitted.", // ProjectUpdateSubmitted
            3 => $"The update for Project {projectRef} has been returned.", // ProjectUpdateReturned
            4 => $"The update for Project {projectRef} was approved.", // ProjectUpdateApproved
            5 => "A customized notification was sent.", // Custom
            6 => $"A Project proposal {projectRef} was submitted for review.", // ProjectSubmitted
            7 => $"A Project proposal {projectRef} was approved for the 5-year list.", // ProjectApproved
            8 => $"A Project proposal {projectRef} was returned for additional information.", // ProjectReturned
            _ => "Notification sent."
        };
    }

    public static async Task<List<StewardshipAreaItem>> ListStewardshipRegionsAsync(WADNRDbContext dbContext)
    {
        return await dbContext.DNRUplandRegions
            .AsNoTracking()
            .OrderBy(r => r.DNRUplandRegionName)
            .Select(r => new StewardshipAreaItem
            {
                ID = r.DNRUplandRegionID,
                Name = r.DNRUplandRegionName
            })
            .ToListAsync();
    }

    public static async Task<PersonDetail?> UpdateStewardshipAreasAsync(WADNRDbContext dbContext, int personID, PersonStewardshipAreasUpsertRequest request)
    {
        // Delete existing steward regions for this person
        var existing = await dbContext.PersonStewardRegions
            .Where(x => x.PersonID == personID)
            .ToListAsync();
        dbContext.PersonStewardRegions.RemoveRange(existing);

        // Insert new ones
        foreach (var regionID in request.DNRUplandRegionIDs)
        {
            dbContext.PersonStewardRegions.Add(new PersonStewardRegion
            {
                PersonID = personID,
                DNRUplandRegionID = regionID
            });
        }

        await dbContext.SaveChangesAsync();

        return await GetByIDAsDetailAsync(dbContext, personID);
    }

    public static async Task<PersonDetail?> UpdateClaims(WADNRDbContext dbContext, ClaimsPrincipal claimsPrincipal)
    {
        const int wadnrOrganizationID = 4704;

        int? personID = null;
        var globalID = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Sub)?.Value;
        if (!string.IsNullOrEmpty(globalID))
        {
            personID = await dbContext.People.AsNoTracking().Where(x => x.GlobalID == globalID).Select(x => x.PersonID).SingleOrDefaultAsync();
        }

        // Detect if this is an enterprise login (Entra ID) from the sub claim.
        // Enterprise connections (Entra) contain "|waad|" in the sub claim.
        // All other connections (auth0, social, etc.) are treated as public users.
        var isEnterpriseUser = !string.IsNullOrEmpty(globalID) && globalID.Contains("|waad|");

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
                IsUser = true,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                ReceiveSupportEmails = false,
            };

            if (isEnterpriseUser)
            {
                // Entra ID users: Normal role + WA DNR organization
                var personRole = new PersonRole { Person = person, RoleID = (int)RoleEnum.Normal };
                person.OrganizationID = wadnrOrganizationID;
                dbContext.PersonRoles.Add(personRole);
            }
            else
            {
                // Public users: Unassigned role
                var personRole = new PersonRole { Person = person, RoleID = (int)RoleEnum.Unassigned };
                dbContext.PersonRoles.Add(personRole);
            }

            dbContext.People.Add(person);
        }

        var firstName = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.GivenName)?.Value;
        var lastName = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.FamilyName)?.Value;

        if (!string.IsNullOrEmpty(globalID))
        {
            person.GlobalID = globalID;
            person.IsUser = true;
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

        person.LastActivityDate = DateTime.UtcNow;

        await dbContext.SaveChangesWithNoAuditingAsync();
        await dbContext.Entry(person).ReloadAsync();

        return await GetByIDAsDetailAsync(dbContext, person.PersonID);
    }

    public static async Task<(string ApiKey, DateTime GeneratedDate)> GenerateApiKeyAsync(WADNRDbContext dbContext, int personID)
    {
        var person = await dbContext.People.SingleAsync(p => p.PersonID == personID);
        person.ApiKey = Guid.NewGuid();
        person.ApiKeyGeneratedDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return (person.ApiKey.Value.ToString(), person.ApiKeyGeneratedDate.Value);
    }

    public static async Task<(string? ApiKey, DateTime? GeneratedDate)> GetApiKeyByPersonIDAsync(WADNRDbContext dbContext, int personID)
    {
        var result = await dbContext.People
            .Where(p => p.PersonID == personID)
            .Select(p => new { p.ApiKey, p.ApiKeyGeneratedDate })
            .SingleOrDefaultAsync();
        return (result?.ApiKey?.ToString(), result?.ApiKeyGeneratedDate);
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

    public static bool CanViewLandownerInfo(this PersonDetail? user) =>
        user != null &&
        (user.HasElevatedProjectAccess() ||
         (user.SupplementalRoleList?.Any(r => r.RoleID == (int)RoleEnum.CanViewLandownerInfo) ?? false));
}