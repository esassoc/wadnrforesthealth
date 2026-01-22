using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class TaxonomyTrunks
{
    public static async Task<List<TaxonomyTrunkLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
        => await dbContext.TaxonomyTrunks
            .AsNoTracking()
            .OrderBy(x => x.TaxonomyTrunkSortOrder)
            .ThenBy(x => x.TaxonomyTrunkName)
            .Select(x => new TaxonomyTrunkLookupItem
            {
                TaxonomyTrunkID = x.TaxonomyTrunkID,
                TaxonomyTrunkName = x.TaxonomyTrunkName
            })
            .ToListAsync();
}
