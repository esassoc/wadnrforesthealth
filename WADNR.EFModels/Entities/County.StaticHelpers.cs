using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Counties
{
    public static async Task<List<CountyGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.Counties
            .AsNoTracking()
            .OrderBy(x => x.CountyName)
            .Select(CountyProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<CountyDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int countyID)
    {
        var entity = await dbContext.Counties
            .AsNoTracking()
            .Where(x => x.CountyID == countyID)
            .Select(CountyProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<CountyDetail?> CreateAsync(WADNRDbContext dbContext, CountyUpsertRequest dto, int callingPersonID)
    {
        var entity = new County
        {
            CountyName = dto.CountyName,
            StateProvinceID = dto.StateProvinceID
        };
        dbContext.Counties.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.CountyID);
    }

    public static async Task<CountyDetail?> UpdateAsync(WADNRDbContext dbContext, int countyID, CountyUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.Counties
            .FirstAsync(x => x.CountyID == countyID);

        entity.CountyName = dto.CountyName;
        entity.StateProvinceID = dto.StateProvinceID;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.CountyID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int countyID)
    {
        var deletedCount = await dbContext.Counties
            .Where(x => x.CountyID == countyID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}
