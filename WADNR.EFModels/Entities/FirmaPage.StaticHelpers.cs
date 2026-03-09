using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FirmaPages
{
    public static async Task<FirmaPageDetail?> GetByFirmaPageTypeAsDetailAsync(WADNRDbContext dbContext, int firmaPageTypeID)
    {
        var entity = await dbContext.FirmaPages.AsNoTracking().Where(x => x.FirmaPageTypeID == firmaPageTypeID).Select(FirmaPageProjections.AsDetail).SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<FirmaPageDetail?> UpdateAsync(WADNRDbContext dbContext, int firmaPageTypeID, FirmaPageUpsertRequest upsertRequest)
    {
        var firmaPage = await dbContext.FirmaPages
            .SingleOrDefaultAsync(x => x.FirmaPageTypeID == firmaPageTypeID);

        if (firmaPage == null)
        {
            return null;
        }

        firmaPage.FirmaPageContent = upsertRequest.FirmaPageContent;
        await dbContext.SaveChangesAsync();

        return await GetByFirmaPageTypeAsDetailAsync(dbContext, firmaPageTypeID);
    }

    public static async Task<List<FirmaPageGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var items = await dbContext.FirmaPages
            .AsNoTracking()
            .Select(FirmaPageProjections.AsGridRow)
            .ToListAsync();

        foreach (var item in items)
        {
            if (FirmaPageType.AllLookupDictionary.TryGetValue(item.FirmaPageTypeID, out var firmaPageType))
            {
                item.FirmaPageTypeName = firmaPageType.FirmaPageTypeName;
                item.FirmaPageTypeDisplayName = firmaPageType.FirmaPageTypeDisplayName;
                item.FirmaPageRenderTypeID = firmaPageType.FirmaPageRenderTypeID;
                if (FirmaPageRenderType.AllLookupDictionary.TryGetValue(firmaPageType.FirmaPageRenderTypeID, out var renderType))
                {
                    item.FirmaPageRenderTypeDisplayName = renderType.FirmaPageRenderTypeDisplayName;
                }
            }
        }

        return items.OrderBy(x => x.FirmaPageTypeDisplayName).ToList();
    }
}