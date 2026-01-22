using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class TaxonomyBranches
{
    public static async Task<List<TaxonomyBranchGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
        => await TaxonomyBranchProjections.AsGridRow(dbContext.TaxonomyBranches.AsNoTracking())
            .OrderBy(x => x.TaxonomyTrunk.TaxonomyTrunkName)
            .ThenBy(x => x.TaxonomyBranchSortOrder)
            .ThenBy(x => x.TaxonomyBranchName)
            .ToListAsync();

    public static async Task<TaxonomyBranchDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int taxonomyBranchID)
        => await TaxonomyBranchProjections.AsDetail(
                dbContext.TaxonomyBranches.AsNoTracking().Where(x => x.TaxonomyBranchID == taxonomyBranchID))
            .SingleOrDefaultAsync();

    public static async Task<List<TaxonomyBranchLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
        => await TaxonomyBranchProjections.AsLookupItem(dbContext.TaxonomyBranches.AsNoTracking())
            .OrderBy(x => x.TaxonomyBranchName)
            .ToListAsync();

    public static async Task<TaxonomyBranchDetail?> CreateAsync(WADNRDbContext dbContext, TaxonomyBranchUpsertRequest dto)
    {
        var entity = new TaxonomyBranch
        {
            TaxonomyTrunkID = dto.TaxonomyTrunkID,
            TaxonomyBranchName = dto.TaxonomyBranchName,
            TaxonomyBranchDescription = dto.TaxonomyBranchDescription,
            TaxonomyBranchCode = dto.TaxonomyBranchCode,
            ThemeColor = dto.ThemeColor,
            TaxonomyBranchSortOrder = dto.TaxonomyBranchSortOrder
        };
        dbContext.TaxonomyBranches.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.TaxonomyBranchID);
    }

    public static async Task<TaxonomyBranchDetail?> UpdateAsync(WADNRDbContext dbContext, int taxonomyBranchID, TaxonomyBranchUpsertRequest dto)
    {
        var entity = await dbContext.TaxonomyBranches
            .FirstAsync(x => x.TaxonomyBranchID == taxonomyBranchID);

        entity.TaxonomyTrunkID = dto.TaxonomyTrunkID;
        entity.TaxonomyBranchName = dto.TaxonomyBranchName;
        entity.TaxonomyBranchDescription = dto.TaxonomyBranchDescription;
        entity.TaxonomyBranchCode = dto.TaxonomyBranchCode;
        entity.ThemeColor = dto.ThemeColor;
        entity.TaxonomyBranchSortOrder = dto.TaxonomyBranchSortOrder;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.TaxonomyBranchID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int taxonomyBranchID)
    {
        var deletedCount = await dbContext.TaxonomyBranches
            .Where(x => x.TaxonomyBranchID == taxonomyBranchID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<ProjectGridRow>> ListProjectsAsGridRowAsync(WADNRDbContext dbContext, int taxonomyBranchID)
    {
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectType.TaxonomyBranchID == taxonomyBranchID)
            .Where(Projects.IsActiveProjectExpr)
            .OrderBy(p => p.ProjectName)
            .Select(ProjectProjections.AsGridRow)
            .ToListAsync();

        return projects;
    }
}
