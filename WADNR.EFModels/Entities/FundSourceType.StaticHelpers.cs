using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceTypes
{
    public static async Task<List<FundSourceTypeLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FundSourceTypes.AsNoTracking()
            .OrderBy(x => x.FundSourceTypeName)
            .Select(FundSourceTypeProjections.AsLookupItem)
            .ToListAsync();
    }
}
