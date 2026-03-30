using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FederalFundCodes
{
    public static async Task<List<FederalFundCodeLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FederalFundCodes.AsNoTracking()
            .OrderBy(x => x.FederalFundCodeAbbrev)
            .Select(FederalFundCodeProjections.AsLookupItem)
            .ToListAsync();
    }
}
