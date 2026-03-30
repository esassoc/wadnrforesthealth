using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectTags
{
    public static async Task<List<TagLookupItem>> ListForProjectAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.ProjectTags
            .AsNoTracking()
            .Where(pt => pt.ProjectID == projectID)
            .Select(pt => new TagLookupItem
            {
                TagID = pt.TagID,
                TagName = pt.Tag.TagName
            })
            .OrderBy(t => t.TagName)
            .ToListAsync();
    }

    public static async Task<List<TagLookupItem>> SaveAllAsync(WADNRDbContext dbContext, int projectID, ProjectTagSaveRequest request)
    {
        var existing = await dbContext.ProjectTags
            .Where(pt => pt.ProjectID == projectID)
            .ToListAsync();

        var requestedTagIDs = request.TagIDs.ToHashSet();
        var existingTagIDs = existing.Select(pt => pt.TagID).ToHashSet();

        // Delete tags not in request
        var toDelete = existing.Where(pt => !requestedTagIDs.Contains(pt.TagID)).ToList();
        dbContext.ProjectTags.RemoveRange(toDelete);

        // Add new tags
        foreach (var tagID in requestedTagIDs.Except(existingTagIDs))
        {
            dbContext.ProjectTags.Add(new ProjectTag
            {
                ProjectID = projectID,
                TagID = tagID
            });
        }

        await dbContext.SaveChangesAsync();

        return await ListForProjectAsync(dbContext, projectID);
    }
}
