using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class TaxonomyTrunks
{
    public static async Task<List<TaxonomyTrunkGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
        => await TaxonomyTrunkProjections.AsGridRow(dbContext.TaxonomyTrunks.AsNoTracking())
            .OrderBy(x => x.TaxonomyTrunkSortOrder)
            .ThenBy(x => x.TaxonomyTrunkName)
            .ToListAsync();

    public static async Task<TaxonomyTrunkDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int taxonomyTrunkID)
        => await TaxonomyTrunkProjections.AsDetail(
                dbContext.TaxonomyTrunks.AsNoTracking().Where(x => x.TaxonomyTrunkID == taxonomyTrunkID))
            .SingleOrDefaultAsync();

    public static async Task<List<TaxonomyTrunkLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
        => await TaxonomyTrunkProjections.AsLookupItem(dbContext.TaxonomyTrunks.AsNoTracking())
            .OrderBy(x => x.TaxonomyTrunkName)
            .ToListAsync();

    public static async Task<TaxonomyTrunkDetail?> CreateAsync(WADNRDbContext dbContext, TaxonomyTrunkUpsertRequest dto)
    {
        var entity = new TaxonomyTrunk
        {
            TaxonomyTrunkName = dto.TaxonomyTrunkName,
            TaxonomyTrunkDescription = dto.TaxonomyTrunkDescription,
            TaxonomyTrunkCode = dto.TaxonomyTrunkCode,
            ThemeColor = dto.ThemeColor,
            TaxonomyTrunkSortOrder = dto.TaxonomyTrunkSortOrder
        };
        dbContext.TaxonomyTrunks.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.TaxonomyTrunkID);
    }

    public static async Task<TaxonomyTrunkDetail?> UpdateAsync(WADNRDbContext dbContext, int taxonomyTrunkID, TaxonomyTrunkUpsertRequest dto)
    {
        var entity = await dbContext.TaxonomyTrunks
            .FirstAsync(x => x.TaxonomyTrunkID == taxonomyTrunkID);

        entity.TaxonomyTrunkName = dto.TaxonomyTrunkName;
        entity.TaxonomyTrunkDescription = dto.TaxonomyTrunkDescription;
        entity.TaxonomyTrunkCode = dto.TaxonomyTrunkCode;
        entity.ThemeColor = dto.ThemeColor;
        entity.TaxonomyTrunkSortOrder = dto.TaxonomyTrunkSortOrder;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.TaxonomyTrunkID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int taxonomyTrunkID)
    {
        var deletedCount = await dbContext.TaxonomyTrunks
            .Where(x => x.TaxonomyTrunkID == taxonomyTrunkID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<ProjectGridRow>> ListProjectsAsGridRowAsync(WADNRDbContext dbContext, int taxonomyTrunkID)
    {
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectType.TaxonomyBranch.TaxonomyTrunkID == taxonomyTrunkID)
            .Where(Projects.IsActiveProjectExpr)
            .OrderBy(p => p.ProjectName)
            .Select(ProjectProjections.AsGridRow)
            .ToListAsync();

        return projects;
    }

    /// <summary>
    /// Lists projects for a taxonomy trunk visible to the calling user.
    /// </summary>
    public static async Task<List<ProjectGridRow>> ListProjectsAsGridRowForUserAsync(
        WADNRDbContext dbContext,
        int taxonomyTrunkID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        return await query
            .Where(p => p.ProjectType.TaxonomyBranch.TaxonomyTrunkID == taxonomyTrunkID)
            .AsNoTracking()
            .OrderBy(p => p.ProjectName)
            .Select(ProjectProjections.AsGridRow)
            .ToListAsync();
    }
}
