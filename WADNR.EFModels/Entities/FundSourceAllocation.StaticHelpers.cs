using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocations
{
    public static async Task<List<FundSourceAllocationGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FundSourceAllocations
            .AsNoTracking()
            .OrderBy(x => x.FundSource.FundSourceNumber)
            .ThenBy(x => x.FundSourceAllocationName)
            .Select(FundSourceAllocationProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<List<FundSourceAllocationGridRow>> ListForFundSourceAsGridRowAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        return await dbContext.FundSourceAllocations
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .OrderBy(x => x.FundSourceAllocationName)
            .Select(FundSourceAllocationProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<FundSourceAllocationDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        var detail = await dbContext.FundSourceAllocations
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationProjections.AsDetail)
            .SingleOrDefaultAsync();

        // Resolve Division name client-side (static lookup table can't be used in EF projection)
        if (detail?.DivisionID != null && Division.AllLookupDictionary.TryGetValue(detail.DivisionID.Value, out var division))
        {
            detail.DivisionName = division.DivisionDisplayName;
        }

        return detail;
    }

    public static async Task<List<FundSourceAllocationLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FundSourceAllocations
            .AsNoTracking()
            .OrderBy(x => x.FundSource.FundSourceNumber)
            .ThenBy(x => x.FundSourceAllocationName)
            .Select(FundSourceAllocationProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<List<FundSourceAllocationDNRUplandRegionDetailGridRow>> ListByDnrUplandRegionActiveAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {
        return await dbContext.FundSourceAllocations
            .Include(x => x.FundSource)
            .Where(x => x.DNRUplandRegionID == dnrUplandRegionID && x.FundSource.FundSourceStatusID == (int)FundSourceStatusEnum.Active)
            .OrderByDescending(x => x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityNumber : 0)
            .Select(FundSourceAllocationProjections.AsDnrUplandRegionDetailGridRow)
            .ToListAsync();
    }
}
