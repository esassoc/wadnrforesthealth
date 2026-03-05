using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.PriorityLandscape;

namespace WADNR.EFModels.Entities;

public static class PriorityLandscapes
{
    public static async Task<List<PriorityLandscapeGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.PriorityLandscapes
            .AsNoTracking()
            .OrderBy(x => x.PriorityLandscapeName)
            .Select(PriorityLandscapeProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<PriorityLandscapeDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int priorityLandscapeID)
    {
        var entity = await dbContext.PriorityLandscapes
            .AsNoTracking()
            .Where(x => x.PriorityLandscapeID == priorityLandscapeID)
            .Select(PriorityLandscapeProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<PriorityLandscapeDetail?> CreateAsync(WADNRDbContext dbContext, PriorityLandscapeUpsertRequest dto, int callingPersonID)
    {
        var entity = new PriorityLandscape
        {
            PriorityLandscapeName = dto.PriorityLandscapeName,
            PriorityLandscapeDescription = dto.PriorityLandscapeDescription,
            PlanYear = dto.PlanYear,
            PriorityLandscapeCategoryID = dto.PriorityLandscapeCategoryID,
            PriorityLandscapeExternalResources = dto.PriorityLandscapeExternalResources,
            PriorityLandscapeAboveMapText = dto.PriorityLandscapeAboveMapText
        };
        dbContext.PriorityLandscapes.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.PriorityLandscapeID);
    }

    public static async Task<PriorityLandscapeDetail?> UpdateAsync(WADNRDbContext dbContext, int priorityLandscapeID, PriorityLandscapeUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.PriorityLandscapes
            .FirstAsync(x => x.PriorityLandscapeID == priorityLandscapeID);

        entity.PriorityLandscapeName = dto.PriorityLandscapeName;
        entity.PriorityLandscapeDescription = dto.PriorityLandscapeDescription;
        entity.PlanYear = dto.PlanYear;
        entity.PriorityLandscapeCategoryID = dto.PriorityLandscapeCategoryID;
        entity.PriorityLandscapeExternalResources = dto.PriorityLandscapeExternalResources;
        entity.PriorityLandscapeAboveMapText = dto.PriorityLandscapeAboveMapText;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.PriorityLandscapeID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int priorityLandscapeID)
    {
        var deletedCount = await dbContext.PriorityLandscapes
            .Where(x => x.PriorityLandscapeID == priorityLandscapeID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<PriorityLandscapeCategoryLookupItem>> ListCategoriesAsync(WADNRDbContext dbContext)
    {
        return await dbContext.PriorityLandscapeCategories
            .AsNoTracking()
            .OrderBy(x => x.PriorityLandscapeCategoryDisplayName)
            .Select(x => new PriorityLandscapeCategoryLookupItem
            {
                PriorityLandscapeCategoryID = x.PriorityLandscapeCategoryID,
                PriorityLandscapeCategoryDisplayName = x.PriorityLandscapeCategoryDisplayName
            })
            .ToListAsync();
    }

    public static async Task UpdateFileAsync(WADNRDbContext dbContext,
        PriorityLandscapeFileResource entity, string displayName, string? description)
    {
        entity.DisplayName = displayName;
        entity.Description = description;
        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteFileAsync(WADNRDbContext dbContext, PriorityLandscapeFileResource entity)
    {
        dbContext.PriorityLandscapeFileResources.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
