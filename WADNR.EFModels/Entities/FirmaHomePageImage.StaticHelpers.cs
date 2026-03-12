using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FirmaHomePageImages
{
    public static async Task<List<FirmaHomePageImageDetail>> ListAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FirmaHomePageImages
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .Select(FirmaHomePageImageProjections.AsDetail)
            .ToListAsync();
    }

    public static async Task<FirmaHomePageImage> CreateAsync(WADNRDbContext dbContext, int fileResourceID, string caption, int sortOrder)
    {
        var image = new FirmaHomePageImage
        {
            FileResourceID = fileResourceID,
            Caption = caption,
            SortOrder = sortOrder
        };

        dbContext.FirmaHomePageImages.Add(image);
        await dbContext.SaveChangesAsync();
        return image;
    }

    public static async Task UpdateAsync(WADNRDbContext dbContext, FirmaHomePageImage image, FirmaHomePageImageUpsertRequest request)
    {
        image.Caption = request.Caption;
        image.SortOrder = request.SortOrder;
        await dbContext.SaveChangesAsync();
    }

    public static async Task<Guid> DeleteAsync(WADNRDbContext dbContext, FirmaHomePageImage image)
    {
        var fileResourceGuid = image.FileResource.FileResourceGUID;

        dbContext.FirmaHomePageImages.Remove(image);
        dbContext.FileResources.Remove(image.FileResource);
        await dbContext.SaveChangesAsync();

        return fileResourceGuid;
    }

    public static async Task<List<FirmaHomePageImageDetail>> UpdateSortOrderAsync(WADNRDbContext dbContext, List<SortOrderUpdateItem> updates)
    {
        var ids = updates.Select(u => u.ID).ToList();
        var entities = await dbContext.FirmaHomePageImages.Where(x => ids.Contains(x.FirmaHomePageImageID)).ToListAsync();
        foreach (var entity in entities)
        {
            var update = updates.First(u => u.ID == entity.FirmaHomePageImageID);
            entity.SortOrder = update.SortOrder;
        }
        await dbContext.SaveChangesAsync();
        return await ListAsync(dbContext);
    }
}
