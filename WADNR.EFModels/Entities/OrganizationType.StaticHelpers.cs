using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class OrganizationTypes
{
    public static async Task<List<OrganizationTypeLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        var items = await dbContext.OrganizationTypes
            .AsNoTracking()
            .OrderBy(x => x.OrganizationTypeName)
            .Select(x => new OrganizationTypeLookupItem
            {
                OrganizationTypeID = x.OrganizationTypeID,
                OrganizationTypeName = x.OrganizationTypeName
            })
            .ToListAsync();
        return items;
    }
}
