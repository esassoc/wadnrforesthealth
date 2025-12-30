using Microsoft.EntityFrameworkCore;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.EFModels.Entities;

public static class FirmaPages
{
    public static async Task<FirmaPageDetail?> GetByFirmaPageTypeAsDetailAsync(WADNRForestHealthTrackerDbContext dbContext, int firmaPageTypeID)
    {
        var entity = await dbContext.FirmaPages.AsNoTracking().Where(x => x.FirmaPageTypeID == firmaPageTypeID).Select(FirmaPageProjections.AsDetail).SingleOrDefaultAsync();
        return entity;
    }
}