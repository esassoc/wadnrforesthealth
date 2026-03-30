using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class CustomPages
{
    public static async Task<CustomPageDetail?> GetByVanityUrlAsDetailAsync(WADNRDbContext dbContext, string vanityUrl)
    {
        var entity = await dbContext.CustomPages
            .AsNoTracking()
            .Where(x => x.CustomPageVanityUrl == vanityUrl)
            .Select(CustomPageProjections.AsDetail)
            .SingleOrDefaultAsync();

        return entity;
    }

    public static async Task<List<CustomPageMenuItem>> GetByNavigationSectionAsMenuItemsAsync(WADNRDbContext dbContext, int customPageNavigationSectionID)
    {
        var items = await dbContext.CustomPages
            .AsNoTracking()
            .Where(x => x.CustomPageNavigationSectionID == customPageNavigationSectionID)
            .Select(CustomPageProjections.AsMenuItem)
            .ToListAsync();

        return items;
    }

    public static async Task<List<CustomPageMenuItem>> ListAsMenuItemsAsync(WADNRDbContext dbContext)
    {
        var items = await dbContext.CustomPages
            .AsNoTracking()
            .OrderBy(x => x.CustomPageDisplayName)
            .Select(CustomPageProjections.AsMenuItem)
            .ToListAsync();

        return items;
    }

    public static async Task<List<CustomPageGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var items = await dbContext.CustomPages
            .AsNoTracking()
            .OrderBy(x => x.CustomPageDisplayName)
            .Select(CustomPageProjections.AsGridRow)
            .ToListAsync();

        foreach (var item in items)
        {
            if (CustomPageDisplayType.AllLookupDictionary.TryGetValue(item.CustomPageDisplayTypeID, out var displayType))
            {
                item.CustomPageDisplayTypeName = displayType.CustomPageDisplayTypeDisplayName;
            }

            if (CustomPageNavigationSection.AllLookupDictionary.TryGetValue(item.CustomPageNavigationSectionID, out var navSection))
            {
                item.CustomPageNavigationSectionName = navSection.CustomPageNavigationSectionName;
            }
        }

        return items;
    }

    public static async Task<CustomPage?> GetByIDAsync(WADNRDbContext dbContext, int customPageID)
    {
        return await dbContext.CustomPages
            .SingleOrDefaultAsync(x => x.CustomPageID == customPageID);
    }

    public static async Task<CustomPageGridRow?> GetByIDAsGridRowAsync(WADNRDbContext dbContext, int customPageID)
    {
        var item = await dbContext.CustomPages
            .AsNoTracking()
            .Where(x => x.CustomPageID == customPageID)
            .Select(CustomPageProjections.AsGridRow)
            .SingleOrDefaultAsync();

        if (item != null)
        {
            if (CustomPageDisplayType.AllLookupDictionary.TryGetValue(item.CustomPageDisplayTypeID, out var displayType))
            {
                item.CustomPageDisplayTypeName = displayType.CustomPageDisplayTypeDisplayName;
            }

            if (CustomPageNavigationSection.AllLookupDictionary.TryGetValue(item.CustomPageNavigationSectionID, out var navSection))
            {
                item.CustomPageNavigationSectionName = navSection.CustomPageNavigationSectionName;
            }
        }

        return item;
    }

    public static async Task<CustomPageGridRow> CreateAsync(WADNRDbContext dbContext, CustomPageUpsertRequest request)
    {
        var entity = new CustomPage
        {
            CustomPageDisplayName = request.CustomPageDisplayName,
            CustomPageVanityUrl = request.CustomPageVanityUrl,
            CustomPageDisplayTypeID = request.CustomPageDisplayTypeID,
            CustomPageNavigationSectionID = request.CustomPageNavigationSectionID
        };
        dbContext.CustomPages.Add(entity);
        await dbContext.SaveChangesAsync();

        return (await GetByIDAsGridRowAsync(dbContext, entity.CustomPageID))!;
    }

    public static async Task<CustomPageGridRow> UpdateAsync(WADNRDbContext dbContext, CustomPage entity, CustomPageUpsertRequest request)
    {
        entity.CustomPageDisplayName = request.CustomPageDisplayName;
        entity.CustomPageVanityUrl = request.CustomPageVanityUrl;
        entity.CustomPageDisplayTypeID = request.CustomPageDisplayTypeID;
        entity.CustomPageNavigationSectionID = request.CustomPageNavigationSectionID;
        await dbContext.SaveChangesAsync();

        return (await GetByIDAsGridRowAsync(dbContext, entity.CustomPageID))!;
    }

    public static async Task UpdateContentAsync(WADNRDbContext dbContext, CustomPage entity, CustomPageContentUpsertRequest request)
    {
        entity.CustomPageContent = request.CustomPageContent;
        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteAsync(WADNRDbContext dbContext, CustomPage entity)
    {
        dbContext.CustomPages.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
