using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Tags
{
    public static async Task<List<TagGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.Tags
            .AsNoTracking()
            .OrderBy(x => x.TagName)
            .Select(TagProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<TagDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int tagID)
    {
        var entity = await dbContext.Tags
            .AsNoTracking()
            .Where(x => x.TagID == tagID)
            .Select(TagProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<TagDetail?> CreateAsync(WADNRDbContext dbContext, TagUpsertRequest dto)
    {
        var entity = new Tag
        {
            TagName = dto.TagName,
            TagDescription = dto.TagDescription
        };
        dbContext.Tags.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.TagID);
    }

    public static async Task<TagDetail?> UpdateAsync(WADNRDbContext dbContext, int tagID, TagUpsertRequest dto)
    {
        var entity = await dbContext.Tags
            .FirstAsync(x => x.TagID == tagID);

        entity.TagName = dto.TagName;
        entity.TagDescription = dto.TagDescription;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.TagID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int tagID)
    {
        var deletedCount = await dbContext.Tags
            .Where(x => x.TagID == tagID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}
