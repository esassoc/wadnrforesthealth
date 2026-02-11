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
}
