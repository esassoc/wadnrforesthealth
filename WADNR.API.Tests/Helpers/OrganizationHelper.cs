using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Helper methods for creating and managing test organizations.
/// </summary>
public static class OrganizationHelper
{
    /// <summary>
    /// Creates an organization with minimal required data for testing.
    /// </summary>
    public static async Task<Organization> CreateOrganizationAsync(
        WADNRDbContext dbContext,
        string? name = null,
        int? organizationTypeID = null)
    {
        var orgType = organizationTypeID
            ?? (await dbContext.OrganizationTypes.FirstAsync()).OrganizationTypeID;

        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var organization = new Organization
        {
            OrganizationName = name ?? $"Test Org {uniqueSuffix}",
            OrganizationShortName = $"TST{uniqueSuffix}",
            OrganizationTypeID = orgType,
            IsActive = true,
        };

        dbContext.Organizations.Add(organization);
        await dbContext.SaveChangesWithNoAuditingAsync();

        return organization;
    }

    /// <summary>
    /// Gets an existing organization by ID with fresh data.
    /// </summary>
    public static async Task<Organization?> GetByIDAsync(WADNRDbContext dbContext, int organizationID)
    {
        return await dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrganizationID == organizationID);
    }

    /// <summary>
    /// Deletes an organization and all related data.
    /// </summary>
    public static async Task DeleteOrganizationAsync(WADNRDbContext dbContext, int organizationID)
    {
        // Delete related data first (in FK-safe order)
        // Staging tables
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.OrganizationBoundaryStaging WHERE OrganizationID = {organizationID}");

        // Person stewardship assignments
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.PersonStewardOrganization WHERE OrganizationID = {organizationID}");

        // Project organization relationships
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectOrganization WHERE OrganizationID = {organizationID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectOrganizationUpdate WHERE OrganizationID = {organizationID}");

        // Nullify Person.OrganizationID FK (Person has a nullable FK to Organization)
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE dbo.Person SET OrganizationID = NULL WHERE OrganizationID = {organizationID}");

        // Delete Agreements that reference this organization (cascade will handle AgreementPerson, AgreementProject)
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AgreementPerson WHERE AgreementID IN (SELECT AgreementID FROM dbo.Agreement WHERE OrganizationID = {organizationID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AgreementProject WHERE AgreementID IN (SELECT AgreementID FROM dbo.Agreement WHERE OrganizationID = {organizationID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AgreementFundSourceAllocation WHERE AgreementID IN (SELECT AgreementID FROM dbo.Agreement WHERE OrganizationID = {organizationID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Agreement WHERE OrganizationID = {organizationID}");

        // Delete Programs that reference this organization
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProgramPerson WHERE ProgramID IN (SELECT ProgramID FROM dbo.Program WHERE OrganizationID = {organizationID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectProgram WHERE ProgramID IN (SELECT ProgramID FROM dbo.Program WHERE OrganizationID = {organizationID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectUpdateProgram WHERE ProgramID IN (SELECT ProgramID FROM dbo.Program WHERE OrganizationID = {organizationID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectImportBlockList WHERE ProgramID IN (SELECT ProgramID FROM dbo.Program WHERE OrganizationID = {organizationID})");

        // Delete program notification chain: SentProject -> Sent -> Configuration
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProgramNotificationSentProject WHERE ProgramNotificationSentID IN (SELECT ProgramNotificationSentID FROM dbo.ProgramNotificationSent WHERE ProgramNotificationConfigurationID IN (SELECT ProgramNotificationConfigurationID FROM dbo.ProgramNotificationConfiguration WHERE ProgramID IN (SELECT ProgramID FROM dbo.Program WHERE OrganizationID = {organizationID})))");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProgramNotificationSent WHERE ProgramNotificationConfigurationID IN (SELECT ProgramNotificationConfigurationID FROM dbo.ProgramNotificationConfiguration WHERE ProgramID IN (SELECT ProgramID FROM dbo.Program WHERE OrganizationID = {organizationID}))");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProgramNotificationConfiguration WHERE ProgramID IN (SELECT ProgramID FROM dbo.Program WHERE OrganizationID = {organizationID})");

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Program WHERE OrganizationID = {organizationID}");

        // Finally delete the organization
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Organization WHERE OrganizationID = {organizationID}");
    }

    /// <summary>
    /// Gets an existing organization for testing (does not create).
    /// </summary>
    public static async Task<Organization?> GetFirstOrganizationAsync(WADNRDbContext dbContext)
    {
        return await dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}
