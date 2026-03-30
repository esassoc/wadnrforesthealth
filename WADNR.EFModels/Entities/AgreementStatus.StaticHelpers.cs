using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AgreementStatuses
{
    public static async Task<List<AgreementStatusLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.AgreementStatuses
            .AsNoTracking()
            .Select(AgreementStatusProjections.AsLookupItem)
            .OrderBy(x => x.AgreementStatusName)
            .ToListAsync();
    }
}
