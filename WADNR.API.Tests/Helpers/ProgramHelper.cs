using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;
using ProgramEntity = WADNR.EFModels.Entities.Program;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Helper methods for creating and managing test programs.
/// </summary>
public static class ProgramHelper
{
    /// <summary>
    /// Creates a program with minimal required data for testing.
    /// </summary>
    public static async Task<ProgramEntity> CreateProgramAsync(
        WADNRDbContext dbContext,
        int createPersonID,
        string? name = null,
        int? organizationID = null)
    {
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var orgID = organizationID ?? (await dbContext.Organizations.FirstAsync()).OrganizationID;

        var program = new ProgramEntity
        {
            ProgramName = name ?? $"Test Program {uniqueSuffix}",
            ProgramShortName = $"TST{uniqueSuffix}",
            OrganizationID = orgID,
            ProgramIsActive = true,
            ProgramCreateDate = DateTime.UtcNow,
            ProgramCreatePersonID = createPersonID,
            IsDefaultProgramForImportOnly = false,
        };

        dbContext.Programs.Add(program);
        await dbContext.SaveChangesWithNoAuditingAsync();

        return program;
    }

    /// <summary>
    /// Gets an existing program by ID with fresh data.
    /// </summary>
    public static async Task<ProgramEntity?> GetByIDAsync(WADNRDbContext dbContext, int programID)
    {
        return await dbContext.Programs
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProgramID == programID);
    }

    /// <summary>
    /// Deletes a program and all related data.
    /// </summary>
    public static async Task DeleteProgramAsync(WADNRDbContext dbContext, int programID)
    {
        // Use the production delete method which handles the full cascade
        dbContext.ChangeTracker.Clear();
        await Programs.DeleteAsync(dbContext, programID);
    }

    /// <summary>
    /// Gets an existing program for testing (does not create).
    /// </summary>
    public static async Task<ProgramEntity?> GetFirstProgramAsync(WADNRDbContext dbContext)
    {
        return await dbContext.Programs
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Adds an editor to a program.
    /// </summary>
    public static async Task AddEditorAsync(
        WADNRDbContext dbContext,
        int programID,
        int personID)
    {
        dbContext.ProgramPeople.Add(new ProgramPerson
        {
            ProgramID = programID,
            PersonID = personID
        });
        await dbContext.SaveChangesWithNoAuditingAsync();
    }
}
