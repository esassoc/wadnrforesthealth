using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProgramIndex;

namespace WADNR.EFModels.Entities;

public static class ProgramIndices
{
    public static async Task<List<ProgramIndexGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ProgramIndices
            .AsNoTracking()
            .OrderBy(x => x.Biennium)
            .ThenBy(x => x.ProgramIndexCode)
            .Select(ProgramIndexProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<List<ProgramIndexGridRow>> ListForBienniumAsGridRowAsync(WADNRDbContext dbContext, int biennium)
    {
        return await dbContext.ProgramIndices
            .AsNoTracking()
            .Where(x => x.Biennium == biennium)
            .OrderBy(x => x.ProgramIndexCode)
            .Select(ProgramIndexProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<ProgramIndexDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int programIndexID)
    {
        return await dbContext.ProgramIndices
            .AsNoTracking()
            .Where(x => x.ProgramIndexID == programIndexID)
            .Select(ProgramIndexProjections.AsDetail)
            .SingleOrDefaultAsync();
    }

    public static async Task<List<ProgramIndexLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ProgramIndices
            .AsNoTracking()
            .OrderBy(x => x.ProgramIndexCode)
            .Select(ProgramIndexProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<List<ProgramIndexLookupItem>> ListForBienniumAsLookupItemAsync(WADNRDbContext dbContext, int biennium)
    {
        return await dbContext.ProgramIndices
            .AsNoTracking()
            .Where(x => x.Biennium == biennium)
            .OrderBy(x => x.ProgramIndexCode)
            .Select(ProgramIndexProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<List<ProgramIndexLookupItem>> SearchAsLookupItemAsync(WADNRDbContext dbContext, string searchTerm, int? biennium = null)
    {
        var query = dbContext.ProgramIndices.AsNoTracking();

        if (biennium.HasValue)
        {
            query = query.Where(x => x.Biennium == biennium.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(x =>
                x.ProgramIndexCode.ToLower().Contains(term) ||
                x.ProgramIndexTitle.ToLower().Contains(term));
        }

        return await query
            .OrderBy(x => x.ProgramIndexCode)
            .Take(20)
            .Select(ProgramIndexProjections.AsLookupItem)
            .ToListAsync();
    }
}
