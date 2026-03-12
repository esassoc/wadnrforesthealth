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

    public static async Task<TagDetail?> BulkTagProjectsAsync(WADNRDbContext dbContext, BulkTagProjectsRequest dto)
    {
        // Case-insensitive find-or-create
        var tag = await dbContext.Tags
            .FirstOrDefaultAsync(x => x.TagName.ToLower() == dto.TagName.ToLower());

        if (tag == null)
        {
            tag = new Tag { TagName = dto.TagName };
            dbContext.Tags.Add(tag);
            await dbContext.SaveChangesAsync();
        }

        // Get existing ProjectTag rows to avoid AK violation
        var existingProjectIDs = await dbContext.ProjectTags
            .Where(pt => pt.TagID == tag.TagID && dto.ProjectIDs.Contains(pt.ProjectID))
            .Select(pt => pt.ProjectID)
            .ToListAsync();

        var newProjectIDs = dto.ProjectIDs.Except(existingProjectIDs).ToList();

        foreach (var projectID in newProjectIDs)
        {
            dbContext.ProjectTags.Add(new ProjectTag
            {
                TagID = tag.TagID,
                ProjectID = projectID
            });
        }

        if (newProjectIDs.Count > 0)
        {
            await dbContext.SaveChangesAsync();
        }

        return await GetByIDAsDetailAsync(dbContext, tag.TagID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int tagID)
    {
        await dbContext.ProjectTags
            .Where(x => x.TagID == tagID)
            .ExecuteDeleteAsync();

        var deletedCount = await dbContext.Tags
            .Where(x => x.TagID == tagID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}
