using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class CustomPages
{
    public static async Task<CustomPageDetail?> GetByCustomPageIDAsDetailAsync(WADNRDbContext dbContext, int customPageID)
    {
        var entity = await dbContext.CustomPages
            .AsNoTracking()
            .Where(x => x.CustomPageID == customPageID)
            .Select(CustomPageProjections.AsDetail)
            .SingleOrDefaultAsync();

        return entity;
    }
}
