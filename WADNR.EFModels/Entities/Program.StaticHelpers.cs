using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Programs
{
    public static async Task<List<ProgramGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.Programs.AsNoTracking().Select(ProgramProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<ProgramDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int programID)
    {
        var entity = await dbContext.Programs.AsNoTracking().Where(x => x.ProgramID == programID).Select(ProgramProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<ProgramDetail?> CreateAsync(WADNRDbContext dbContext, ProgramUpsertRequest dto, int callingPersonID)
    {
        var entity = new Program
        {
            OrganizationID = dto.OrganizationID,
            ProgramName = dto.IsDefaultProgramForImportOnly ? null : dto.ProgramName,
            ProgramShortName = dto.IsDefaultProgramForImportOnly ? null : dto.ProgramShortName,
            ProgramPrimaryContactPersonID = dto.ProgramPrimaryContactPersonID,
            ProgramIsActive = dto.ProgramIsActive,
            IsDefaultProgramForImportOnly = dto.IsDefaultProgramForImportOnly,
            ProgramNotes = dto.ProgramNotes,
            ProgramFileResourceID = dto.ProgramFileResourceID,
            ProgramExampleGeospatialUploadFileResourceID = dto.ProgramExampleGeospatialUploadFileResourceID,
            ProgramCreateDate = DateTime.UtcNow,
            ProgramCreatePersonID = callingPersonID
        };
        dbContext.Programs.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProgramID);
    }

    public static async Task<ProgramDetail?> UpdateAsync(WADNRDbContext dbContext, int programID, ProgramUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.Programs
            .Include(x => x.Organization)
            .FirstAsync(x => x.ProgramID == programID);

        // map fields from dto to entity
        entity.OrganizationID = dto.OrganizationID;
        entity.ProgramName = dto.IsDefaultProgramForImportOnly ? null : dto.ProgramName;
        entity.ProgramShortName = dto.IsDefaultProgramForImportOnly ? null : dto.ProgramShortName;
        entity.ProgramPrimaryContactPersonID = dto.ProgramPrimaryContactPersonID;
        entity.ProgramIsActive = dto.ProgramIsActive;
        entity.IsDefaultProgramForImportOnly = dto.IsDefaultProgramForImportOnly;
        entity.ProgramNotes = dto.ProgramNotes;
        entity.ProgramFileResourceID = dto.ProgramFileResourceID;
        entity.ProgramExampleGeospatialUploadFileResourceID = dto.ProgramExampleGeospatialUploadFileResourceID;

        entity.ProgramLastUpdatedDate = DateTime.UtcNow;
        entity.ProgramLastUpdatedByPersonID = callingPersonID;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProgramID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int programID)
    {
        var deletedCount = await dbContext.Programs
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static bool IsProgramNameUnique(IEnumerable<Program> programs, string programName, int currentProgramID)
    {
        var program =
            programs.SingleOrDefault(x => x.ProgramID != currentProgramID && string.Equals(x.ProgramName, programName, StringComparison.InvariantCultureIgnoreCase));
        return program == null;
    }
}