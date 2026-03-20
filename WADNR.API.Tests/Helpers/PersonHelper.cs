using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Helper methods for creating and managing test people/contacts.
/// </summary>
public static class PersonHelper
{
    /// <summary>
    /// Creates a contact (person without login) with minimal required data.
    /// </summary>
    public static async Task<Person> CreateContactAsync(
        WADNRDbContext dbContext,
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        int? organizationID = null)
    {
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var person = new Person
        {
            FirstName = firstName ?? $"TestFirst{uniqueSuffix}",
            LastName = lastName ?? $"TestLast{uniqueSuffix}",
            Email = email ?? $"test{uniqueSuffix}@example.com",
            OrganizationID = organizationID ?? (await dbContext.Organizations.FirstAsync()).OrganizationID,
            IsActive = true,
            CreateDate = DateTime.UtcNow,
            // No GlobalID means this is a contact, not a full user
        };

        dbContext.People.Add(person);
        await dbContext.SaveChangesWithNoAuditingAsync();

        return person;
    }

    /// <summary>
    /// Creates a full user (person with login capability) with minimal required data.
    /// </summary>
    public static async Task<Person> CreateUserAsync(
        WADNRDbContext dbContext,
        RoleEnum baseRole = RoleEnum.Normal,
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        int? organizationID = null)
    {
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var person = new Person
        {
            FirstName = firstName ?? $"TestUser{uniqueSuffix}",
            LastName = lastName ?? $"TestLast{uniqueSuffix}",
            Email = email ?? $"testuser{uniqueSuffix}@example.com",
            OrganizationID = organizationID ?? (await dbContext.Organizations.FirstAsync()).OrganizationID,
            IsActive = true,
            CreateDate = DateTime.UtcNow,
            GlobalID = Guid.NewGuid().ToString(),
            IsUser = true, // User (not a contact)
        };

        dbContext.People.Add(person);
        await dbContext.SaveChangesWithNoAuditingAsync();

        // Add base role
        dbContext.PersonRoles.Add(new PersonRole
        {
            PersonID = person.PersonID,
            RoleID = (int)baseRole
        });
        await dbContext.SaveChangesWithNoAuditingAsync();

        return person;
    }

    /// <summary>
    /// Adds a supplemental role to an existing person.
    /// </summary>
    public static async Task AddSupplementalRoleAsync(
        WADNRDbContext dbContext,
        int personID,
        RoleEnum supplementalRole)
    {
        dbContext.PersonRoles.Add(new PersonRole
        {
            PersonID = personID,
            RoleID = (int)supplementalRole
        });
        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Gets an existing person by ID with fresh data.
    /// </summary>
    public static async Task<Person?> GetByIDAsync(WADNRDbContext dbContext, int personID)
    {
        return await dbContext.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonID == personID);
    }

    /// <summary>
    /// Deletes a person and all related data. Uses direct SQL for efficiency.
    /// </summary>
    public static async Task DeletePersonAsync(WADNRDbContext dbContext, int personID)
    {
        // Delete related data in FK-safe order
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.PersonRole WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.PersonStewardOrganization WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.PersonStewardRegion WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.PersonStewardTaxonomyBranch WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.PersonAllowedAuthenticator WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectPerson WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectPersonUpdate WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AgreementPerson WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.InteractionEventContact WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Notification WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProgramPerson WHERE PersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.SupportRequestLog WHERE RequestPersonID = {personID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.FundSourceAllocationLikelyPerson WHERE PersonID = {personID}");

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Person WHERE PersonID = {personID}");
    }

    /// <summary>
    /// Gets an existing person for testing (does not create).
    /// </summary>
    public static async Task<Person?> GetFirstPersonAsync(WADNRDbContext dbContext)
    {
        return await dbContext.People
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets the first person with a specific base role.
    /// </summary>
    public static async Task<Person?> GetFirstPersonWithRoleAsync(WADNRDbContext dbContext, RoleEnum role)
    {
        var personID = await dbContext.PersonRoles
            .Where(pr => pr.RoleID == (int)role)
            .Select(pr => pr.PersonID)
            .FirstOrDefaultAsync();

        if (personID == 0) return null;

        return await dbContext.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonID == personID);
    }
}
