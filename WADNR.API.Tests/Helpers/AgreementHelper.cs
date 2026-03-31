using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Helper methods for creating and managing test agreements.
/// </summary>
public static class AgreementHelper
{
    /// <summary>
    /// Creates an agreement with minimal required data for testing.
    /// </summary>
    public static async Task<Agreement> CreateAgreementAsync(
        WADNRDbContext dbContext,
        string? title = null,
        int? agreementTypeID = null,
        int? agreementStatusID = null,
        int? organizationID = null)
    {
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;

        // Get valid lookup IDs if not provided
        var typeID = agreementTypeID ?? (await dbContext.AgreementTypes.FirstAsync()).AgreementTypeID;
        var statusID = agreementStatusID ?? (await dbContext.AgreementStatuses.FirstAsync()).AgreementStatusID;
        var orgID = organizationID ?? (await dbContext.Organizations.FirstAsync()).OrganizationID;

        var agreement = new Agreement
        {
            AgreementTitle = title ?? $"Test Agreement {uniqueSuffix}",
            AgreementNumber = $"AGR-{uniqueSuffix}",
            AgreementTypeID = typeID,
            AgreementStatusID = statusID,
            OrganizationID = orgID,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
        };

        dbContext.Agreements.Add(agreement);
        await dbContext.SaveChangesWithNoAuditingAsync();

        return agreement;
    }

    /// <summary>
    /// Creates an agreement with a specific organization.
    /// </summary>
    public static async Task<Agreement> CreateAgreementForOrganizationAsync(
        WADNRDbContext dbContext,
        int organizationID,
        string? title = null)
    {
        return await CreateAgreementAsync(dbContext, title, organizationID: organizationID);
    }

    /// <summary>
    /// Gets an existing agreement by ID with fresh data.
    /// </summary>
    public static async Task<Agreement?> GetByIDAsync(WADNRDbContext dbContext, int agreementID)
    {
        return await dbContext.Agreements
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AgreementID == agreementID);
    }

    /// <summary>
    /// Deletes an agreement and all related data.
    /// </summary>
    public static async Task DeleteAgreementAsync(WADNRDbContext dbContext, int agreementID)
    {
        // Delete related data in FK-safe order
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AgreementPerson WHERE AgreementID = {agreementID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AgreementProject WHERE AgreementID = {agreementID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AgreementFundSourceAllocation WHERE AgreementID = {agreementID}");

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Agreement WHERE AgreementID = {agreementID}");
    }

    /// <summary>
    /// Gets an existing agreement for testing (does not create).
    /// </summary>
    public static async Task<Agreement?> GetFirstAgreementAsync(WADNRDbContext dbContext)
    {
        return await dbContext.Agreements
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Adds a contact to an agreement.
    /// </summary>
    public static async Task AddContactAsync(
        WADNRDbContext dbContext,
        int agreementID,
        int personID,
        int agreementPersonRoleID)
    {
        dbContext.AgreementPeople.Add(new AgreementPerson
        {
            AgreementID = agreementID,
            PersonID = personID,
            AgreementPersonRoleID = agreementPersonRoleID
        });
        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Adds a project to an agreement.
    /// </summary>
    public static async Task AddProjectAsync(
        WADNRDbContext dbContext,
        int agreementID,
        int projectID)
    {
        dbContext.AgreementProjects.Add(new AgreementProject
        {
            AgreementID = agreementID,
            ProjectID = projectID
        });
        await dbContext.SaveChangesWithNoAuditingAsync();
    }
}
