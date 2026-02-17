using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocationPriorities
{
    public static async Task<List<FundSourceAllocationPriorityLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FundSourceAllocationPriorities.AsNoTracking()
            .OrderBy(x => x.FundSourceAllocationPriorityNumber)
            .Select(FundSourceAllocationPriorityProjections.AsLookupItem)
            .ToListAsync();
    }
}
