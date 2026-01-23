using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ClassificationSystem;

namespace WADNR.EFModels.Entities;

public static class ClassificationSystems
{
    public static async Task<List<ClassificationSystemGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ClassificationSystems
            .AsNoTracking()
            .OrderBy(x => x.ClassificationSystemName)
            .Select(ClassificationSystemProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<ClassificationSystemDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int classificationSystemID)
    {
        return await dbContext.ClassificationSystems
            .AsNoTracking()
            .Where(x => x.ClassificationSystemID == classificationSystemID)
            .Select(ClassificationSystemProjections.AsDetail)
            .SingleOrDefaultAsync();
    }

    public static async Task<List<ClassificationSystemLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ClassificationSystems
            .AsNoTracking()
            .OrderBy(x => x.ClassificationSystemName)
            .Select(ClassificationSystemProjections.AsLookupItem)
            .ToListAsync();
    }
}
