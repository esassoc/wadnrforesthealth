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
}
