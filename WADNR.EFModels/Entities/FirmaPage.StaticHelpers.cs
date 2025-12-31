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
}